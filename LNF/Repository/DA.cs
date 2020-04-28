using LNF.CommonTools;
using LNF.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace LNF.Repository
{
    public static class DA
    {
        /// <summary>
        /// Gets an ISession instance that uses the current data access context.
        /// </summary>
        [Obsolete("Move to LNF.Impl")]
        public static ISession Current => ServiceProvider.Current.DataAccess.Session;

        /// <summary>
        /// Initiates a new current session. All subsequent data access actions (DA.Current) will be in one transaction which is committed when this instance is disposed.
        /// </summary>
        /// <returns></returns>
        public static IDisposable StartUnitOfWork() => ServiceProvider.Current.DataAccess.StartUnitOfWork();

        /// <summary>
        /// Create a new DataCommand instance.
        /// </summary>
        /// <param name="type">The CommandType used for selects. Also the default CommandType for updates (can be changed in Update action).</param>
        [Obsolete("Use Session.Command() instead.")]
        public static IDataCommand Command(CommandType type = CommandType.StoredProcedure) => DefaultDataCommand.Create(type);


        //The methods that were previously defined here have been moved to LNF.Repository.ISession
        //I think these methods should only be defined once, so now they are accessed via Current
    }

    public class DefaultDataCommand : DataCommandBase
    {
        private DefaultDataCommand(CommandType type) : base(type) { }

        protected override IUnitOfWorkAdapter GetAdapter() => SQLDBAccess.Create("cnSselData");

        public static DataCommandBase Create(CommandType type = CommandType.StoredProcedure) => new DefaultDataCommand(type);
    }

    public class ReadOnlyDataCommand : DataCommandBase
    {
        private ReadOnlyDataCommand(CommandType type) : base(type) { }

        protected override IUnitOfWorkAdapter GetAdapter() => SQLDBAccess.Create("cnSselDataReadOnly");

        public static DataCommandBase Create(CommandType type = CommandType.StoredProcedure) => new ReadOnlyDataCommand(type);
    }

    public class SessionDataCommand : DataCommandBase
    {
        private SessionDataCommand(CommandType type) : base(type) { }

        protected override IUnitOfWorkAdapter GetAdapter() => throw new NotImplementedException(); //ServiceProvider.Current.DataAccess.Session.GetAdapter();

        public static DataCommandBase Create(CommandType type = CommandType.StoredProcedure) => new SessionDataCommand(type);
    }

    public abstract class DataCommandBase : IDataCommand
    {
        //private Func<UnitOfWorkAdapter> _adap;
        private bool _mapSchema = false;

        //public static DataCommand Create(CommandType type = CommandType.StoredProcedure) => new DataCommand(() => SQLDBAccess.Create("cnSselData"), type);

        //public static DataCommand Create(Func<UnitOfWorkAdapter> fn, CommandType type = CommandType.StoredProcedure) => new DataCommand(fn, type);

        private readonly IDictionary<string, CommandConfiguration> _configs = new Dictionary<string, CommandConfiguration>
        {
            ["select"] = new CommandConfiguration(),
            ["insert"] = new CommandConfiguration(),
            ["update"] = new CommandConfiguration(),
            ["delete"] = new CommandConfiguration()
        };

        protected DataCommandBase(CommandType type)
        {
            //_adap = fn;
            _configs["select"].SetCommandType(type);
            _configs["insert"].SetCommandType(type);
            _configs["update"].SetCommandType(type);
            _configs["delete"].SetCommandType(type);
        }

        protected abstract IUnitOfWorkAdapter GetAdapter();
        //{
        //    if (_adap != null) return _adap();
        //    else throw new Exception("Use the static Create method or override this class.");
        //}

        public IDataCommand Timeout(int value)
        {
            _configs["select"].SetCommandTimeout(value);
            return this;
        }

        public IDataCommand MapSchema()
        {
            _mapSchema = true;
            return this;
        }

        public IDataCommand Param(object parameters)
        {
            _configs["select"].AddParameter(parameters);
            return this;
        }

        public IDataCommand Param(IDictionary<string, object> parameters)
        {
            _configs["select"].AddParameter(parameters);
            return this;
        }

        public IDataCommand Param(string name, object value)
        {
            _configs["select"].AddParameter(name, value);
            return this;
        }

        public IDataCommand Param(string name, object value, ParameterDirection direction)
        {
            _configs["select"].AddParameter(name, value, direction);
            return this;
        }

        public IDataCommand Param(string name, bool test, object value)
        {
            _configs["select"].AddParameter(name, test, value);
            return this;
        }

        public IDataCommand Param(string name, bool test, object v1, object v2)
        {
            _configs["select"].AddParameter(name, test, v1, v2);
            return this;
        }

        public IDataCommand ParamList(string prefix, IEnumerable values)
        {
            _configs["select"].AddParameterList(prefix, values);
            return this;
        }

        public ExecuteFillDataTableResult GetDataTableResult(string commandText)
        {
            var dt = new DataTable();
            return GetDataTableResult(dt, commandText);
        }

        public ExecuteFillDataTableResult GetDataTableResult(DataTable dt, string commandText)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                return new ExecuteFillDataTableResult(dt, adap);
            }
        }

        public ExecuteFillDataSetResult GetDataSetResult(string commandText, string srcTable = null)
        {
            var ds = new DataSet();
            return GetDataSetResult(ds, commandText, srcTable);
        }

        public ExecuteFillDataSetResult GetDataSetResult(DataSet ds, string commandText, string srcTable = null)
        {
            using (var adap = GetSelectAdapter(commandText, ds))
            {
                return new ExecuteFillDataSetResult(ds, adap, srcTable);
            }
        }

        public DataTable FillDataTable(string commandText)
        {
            return GetDataTableResult(commandText).Result;
        }

        public void FillDataTable(DataTable dt, string commandText)
        {
            GetDataTableResult(dt, commandText);
        }

        public DataSet FillDataSet(string commandText)
        {
            return GetDataSetResult(commandText).Result;
        }

        public void FillDataSet(DataSet ds, string commandText)
        {
            GetDataSetResult(ds, commandText);
        }

        public void FillDataSet(DataSet ds, string commandText, string srcTable)
        {
            GetDataSetResult(ds, commandText, srcTable);
        }

        public ExecuteReaderResult ExecuteReader(string commandText)
        {
            var adap = GetSelectAdapter(commandText);
            var result = new ExecuteReaderResult(adap);
            return result;
        }

        public ExecuteScalarResult<T> ExecuteScalar<T>(string commandText)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                return new ExecuteScalarResult<T>(adap);
            }
        }

        public ExecuteNonQueryResult ExecuteNonQuery(string commandText)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                return new ExecuteNonQueryResult(adap);
            }
        }

        public int Update(DataTable dt, Action<UpdateConfiguration> action)
        {
            using (var adap = GetUpdateAdapter(action))
            {
                return adap.Update(dt);
            }
        }

        public int Update(DataRow[] dataRows, Action<UpdateConfiguration> action)
        {
            using (var adap = GetUpdateAdapter(action))
            {
                return adap.Update(dataRows);
            }
        }

        public int Update(DataSet ds, Action<UpdateConfiguration> action)
        {
            using (var adap = GetUpdateAdapter(action))
            {
                return adap.Update(ds);
            }
        }

        public int Update(DataSet ds, string srcTable, Action<UpdateConfiguration> action)
        {
            using (var adap = GetUpdateAdapter(action))
            {
                return adap.Update(ds, srcTable);
            }
        }

        public void Batch(Action<BatchConfiguration> action)
        {
            using (var adap = GetAdapter())
            {
                action(new BatchConfiguration(adap, _configs));
            }
        }

        public T Batch<T>(Func<BatchConfiguration, T> fn)
        {
            using (var adap = GetAdapter())
            {
                return fn(new BatchConfiguration(adap, _configs));
            }
        }

        private IUnitOfWorkAdapter GetSelectAdapter(string commandText)
        {
            var adap = GetAdapter();
            adap.MapTableSchema = _mapSchema;
            _configs["select"].SetCommandText(commandText);
            _configs["select"].Configure(adap.SelectCommand);
            return adap;
        }

        private IUnitOfWorkAdapter GetSelectAdapter(string commandText, DataSet ds)
        {
            var adap = GetSelectAdapter(commandText);
            MapSchema(adap, ds);
            return adap;
        }

        private IUnitOfWorkAdapter GetUpdateAdapter(Action<UpdateConfiguration> action)
        {
            var adap = GetAdapter();

            adap.MapTableSchema = _mapSchema;

            action(new UpdateConfiguration(_configs["insert"], _configs["update"], _configs["delete"]));

            _configs["insert"].Configure(adap.InsertCommand);
            _configs["update"].Configure(adap.UpdateCommand);
            _configs["delete"].Configure(adap.DeleteCommand);

            return adap;
        }

        private void MapSchema(IUnitOfWorkAdapter adap, DataTable dt)
        {
            if (adap.MapTableSchema)
                adap.FillSchema(dt, SchemaType.Source);
        }

        private void MapSchema(IUnitOfWorkAdapter adap, DataSet ds)
        {
            if (adap.MapTableSchema)
                adap.FillSchema(ds, SchemaType.Source);
        }
    }

    public class UpdateConfiguration
    {
        public CommandConfiguration Insert { get; }
        public CommandConfiguration Update { get; }
        public CommandConfiguration Delete { get; }

        internal UpdateConfiguration(CommandConfiguration insert, CommandConfiguration update, CommandConfiguration delete)
        {
            Insert = insert;
            Update = update;
            Delete = delete;
        }
    }

    public class CommandConfiguration
    {
        private string _commandText;
        private int _commandTimeout = 30;
        private CommandType _commandType = CommandType.StoredProcedure;
        private readonly ParameterDefinitionCollection _parameters = new ParameterDefinitionCollection();
        private readonly IDictionary<string, string> _lists = new Dictionary<string, string>();

        public void SetCommandText(string value)
        {
            _commandText = value;
        }

        public void SetCommandTimeout(int value)
        {
            _commandTimeout = value;
        }

        public void SetCommandType(CommandType value)
        {
            _commandType = value;
        }

        /// <summary>
        /// Adds parameters to the select command.
        /// </summary>
        public void AddParameter(object parameters)
        {
            IDictionary<string, object> dict = null;

            if (parameters != null)
                dict = parameters.GetType().GetProperties().ToDictionary(k => k.Name, v => v.GetValue(parameters, null));

            AddParameter(dict);
        }

        /// <summary>
        /// Adds parameters to the select command.
        /// </summary>
        public void AddParameter(IDictionary<string, object> parameters)
        {
            _parameters.Clear();

            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    _parameters.SetParameter(new ParameterDefinition()
                    {
                        Name = kvp.Key,
                        Value = kvp.Value,
                        DbType = DbTypeUtil.GetDbType(kvp.Value)
                    });
                }
            }
        }

        /// <summary>
        /// Adds a parameter to the select command.
        /// </summary>
        public void AddParameter(string name, object value)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                Value = value,
                DbType = DbTypeUtil.GetDbType(value)
            });
        }

        /// <summary>
        /// Adds a parameter to the select command with the specified parameter direction.
        /// </summary>
        public void AddParameter(string name, object value, ParameterDirection direction)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                Value = value,
                DbType = DbTypeUtil.GetDbType(value),
                Direction = direction
            });
        }

        /// <summary>
        /// Adds a parameter to the select command if the condition is true.
        /// </summary>
        public void AddParameter(string name, bool test, object value)
        {
            if (test)
                AddParameter(name, value);
            else
                _parameters.Remove(name);
        }

        /// <summary>
        /// Adds a parameter to the select command using v1 if the condition is true, or v2 if the condition is false.
        /// </summary>
        public void AddParameter(string name, bool test, object v1, object v2)
        {
            if (test)
                AddParameter(name, v1);
            else
                AddParameter(name, v2);
        }

        public void AddParameter(string name, SqlDbType type)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = DbTypeUtil.ConvertToDbType(type)
            });
        }

        public void AddParameter(string name, DbType type)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = type
            });
        }

        public void AddParameter(string name, SqlDbType type, ParameterDirection direction)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = DbTypeUtil.ConvertToDbType(type),
                Direction = direction
            });
        }

        public void AddParameter(string name, DbType type, ParameterDirection direction)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = type,
                Direction = direction
            });
        }

        public void AddParameter(string name, SqlDbType type, int size)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = DbTypeUtil.ConvertToDbType(type),
                Size = size
            });
        }

        public void AddParameter(string name, DbType type, int size)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = type,
                Size = size
            });
        }

        public void AddParameter(string name, SqlDbType type, int size, ParameterDirection direction)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = DbTypeUtil.ConvertToDbType(type),
                Size = size,
                Direction = direction
            });
        }

        public void AddParameter(string name, DbType type, int size, ParameterDirection direction)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = type,
                Size = size,
                Direction = direction
            });
        }

        public void AddParameter(string name, SqlDbType type, int size, string source)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = DbTypeUtil.ConvertToDbType(type),
                Size = size,
                SourceColumn = source
            });
        }

        public void AddParameter(string name, DbType type, int size, string source)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = type,
                Size = size,
                SourceColumn = source
            });
        }

        public void AddParameter(string name, SqlDbType type, int size, string source, ParameterDirection direction)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = DbTypeUtil.ConvertToDbType(type),
                Size = size,
                SourceColumn = source,
                Direction = direction
            });
        }

        public void AddParameter(string name, DbType type, int size, string source, ParameterDirection direction)
        {
            _parameters.SetParameter(new ParameterDefinition()
            {
                Name = name,
                DbType = type,
                Size = size,
                SourceColumn = source,
                Direction = direction
            });
        }

        /// <summary>
        /// Adds parameters to the select command as p1, p2, ...
        /// </summary>
        public void AddParameterList(string prefix, IEnumerable values)
        {
            int i = 0;
            var plist = string.Empty;
            var comma = string.Empty;

            foreach (var v in values)
            {
                var name = prefix + i.ToString();
                AddParameter(name, v);
                plist += comma + "@" + name;
                comma = ",";
                ++i;
            }

            string key = ":" + prefix;

            if (i == 0)
                plist = "NULL"; // results in WHERE SomeColumn IN (NULL), which is valid sql

            if (_lists.ContainsKey(key))
                _lists[key] = plist;
            else
                _lists.Add(key, plist);
        }

        internal void Configure(DbCommand command)
        {
            command.CommandType = _commandType;
            command.CommandText = FormatCommandText();
            command.CommandTimeout = _commandTimeout;

            command.Parameters.Clear();

            foreach (var kvp in _parameters)
            {
                var def = kvp.Value;

                var p = command.CreateParameter();

                p.ParameterName = def.Name;
                p.Value = def.Value;
                p.Direction = def.Direction;
                p.DbType = def.DbType;
                p.Size = def.Size;
                p.SourceColumn = string.IsNullOrEmpty(def.SourceColumn) ? def.Name.TrimStart('@') : def.SourceColumn;

                command.Parameters.Add(p);
            }
        }

        private string FormatCommandText()
        {
            if (string.IsNullOrEmpty(_commandText))
                return string.Empty;

            string result = _commandText;

            foreach (var kvp in _lists)
            {
                //assume lists are always enclosed in parentheses
                result = result.Replace("(" + kvp.Key + ")", "(" + kvp.Value + ")");
            }

            return result;
        }

        internal void Configure(object selectCommand)
        {
            throw new NotImplementedException();
        }
    }

    public class ParameterDefinitionCollection : IEnumerable<KeyValuePair<string, ParameterDefinition>>
    {
        private IDictionary<string, ParameterDefinition> _items = new Dictionary<string, ParameterDefinition>();

        public ParameterDefinition this[string key]
        {
            get { return GetParameter(key); }
            set { SetParameter(key, value); }
        }

        public void SetParameter(ParameterDefinition value)
        {
            SetParameter(value.Name, value);
        }

        private void SetParameter(string key, ParameterDefinition value)
        {
            if (_items.ContainsKey(key))
                _items[key] = value;
            else
                _items.Add(key, value);
        }

        private ParameterDefinition GetParameter(string key)
        {
            if (_items.ContainsKey(key))
                return _items[key];
            else
                return null;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void Remove(string key)
        {
            _items.Remove(key);
        }

        public IEnumerator<KeyValuePair<string, ParameterDefinition>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ParameterDefinition
    {
        public string Name { get; set; }

        public object Value { get; set; }

        // Input is default according to the docs:
        //      https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlparameter.direction?view=netframework-4.7.2
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        // String is default according to the docs:
        //      https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbparameter.dbtype?view=netframework-4.7.2
        public DbType DbType { get; set; } = DbType.String;

        public int Size { get; set; }

        public string SourceColumn { get; internal set; }
    }

    public abstract class ExecuteResult
    {
        private Stopwatch _sw;

        private DbParameterCollection _parameters;

        protected IUnitOfWorkAdapter Adapter { get; }

        protected abstract void Execute();

        public TimeSpan TimeTaken { get; protected set; }

        internal ExecuteResult(IUnitOfWorkAdapter adap)
        {
            Adapter = adap;
        }

        protected void Start()
        {
            _sw = Stopwatch.StartNew();
            Execute();
            Stop();
        }

        private void Stop()
        {
            _sw.Stop();
            _parameters = Adapter.SelectCommand.Parameters;
            TimeTaken = _sw.Elapsed;
        }

        public T GetParamValue<T>(string name, T defval = default(T))
        {
            if (!_parameters.Contains(name))
                return defval;

            var p = (DbParameter)_parameters[name];

            if (p == null)
                return defval;

            return Utility.ConvertTo(p.Value, defval);
        }
    }

    public class ExecuteFillDataTableResult : ExecuteResult
    {
        private IUnitOfWorkAdapter _adap;

        public DataTable Result { get; private set; }

        public ExecuteFillDataTableResult(DataTable dt, IUnitOfWorkAdapter adap) : base(adap)
        {
            _adap = adap;
            Result = dt;
            Start();
        }

        protected override void Execute()
        {
            _adap.Fill(Result);
        }
    }

    public class ExecuteFillDataSetResult : ExecuteResult
    {
        public string SourceTable { get; }
        public DataSet Result { get; private set; }

        public ExecuteFillDataSetResult(DataSet ds, IUnitOfWorkAdapter adap, string srcTable = null) : base(adap)
        {
            Result = ds;
            SourceTable = srcTable;
            Start();
        }

        protected override void Execute()
        {
            // Fill throws an error if srcTable is null so...

            if (string.IsNullOrEmpty(SourceTable))
                Adapter.Fill(Result);
            else
                Adapter.Fill(Result, SourceTable);
        }
    }

    public class ExecuteScalarResult<T> : ExecuteResult
    {
        public T Value { get; private set; }

        internal ExecuteScalarResult(IUnitOfWorkAdapter adap) : base(adap)
        {
            Start();
        }

        protected override void Execute()
        {
            var scalar = Adapter.SelectCommand.ExecuteScalar();
            Value = Utility.ConvertTo(scalar, default(T));
        }
    }

    public class ExecuteNonQueryResult : ExecuteResult
    {
        public int Value { get; private set; }

        internal ExecuteNonQueryResult(IUnitOfWorkAdapter adap) : base(adap)
        {
            Start();
        }

        protected override void Execute()
        {
            Value = Adapter.SelectCommand.ExecuteNonQuery();
        }
    }

    public class ExecuteReaderResult : ExecuteResult, IEnumerable, IDisposable
    {
        private DbDataReader _reader;

        public object this[string name]
        {
            get => _reader[name];
        }

        public object this[int i]
        {
            get => _reader[i];
        }

        public T Value<T>(string key, T defval)
        {
            var val = _reader[key];
            var result = Utility.ConvertTo(val, defval);
            return result;
        }

        internal ExecuteReaderResult(IUnitOfWorkAdapter adap) : base(adap)
        {
            Start();
        }

        protected override void Execute()
        {
            _reader = Adapter.SelectCommand.ExecuteReader();
        }

        public bool Read()
        {
            return _reader.Read();
        }

        public void Close()
        {
            _reader.Close();
        }

        public bool ReadAndClose()
        {
            var result = _reader.Read();
            _reader.Close();
            return result;
        }

        public void Dispose()
        {
            if (!_reader.IsClosed)
                _reader.Close();

            _reader.Dispose();

            Adapter.Dispose();
        }

        public IEnumerator GetEnumerator()
        {
            return _reader.GetEnumerator();
        }
    }

    public class BatchConfiguration
    {
        public BatchCommandConfiguration Select { get; }
        public BatchCommandConfiguration Insert { get; }
        public BatchCommandConfiguration Update { get; }
        public BatchCommandConfiguration Delete { get; }

        internal BatchConfiguration(IUnitOfWorkAdapter adap, IDictionary<string, CommandConfiguration> configs)
        {
            Select = new BatchCommandConfiguration(adap, adap.SelectCommand, configs["select"]);
            Insert = new BatchCommandConfiguration(adap, adap.InsertCommand, configs["insert"]);
            Update = new BatchCommandConfiguration(adap, adap.UpdateCommand, configs["update"]);
            Delete = new BatchCommandConfiguration(adap, adap.DeleteCommand, configs["delete"]);
        }
    }

    public class BatchCommandConfiguration : CommandConfiguration
    {
        private readonly IUnitOfWorkAdapter _adap;
        private readonly DbCommand _command;
        private readonly CommandConfiguration _config;

        internal BatchCommandConfiguration(IUnitOfWorkAdapter adap, DbCommand command, CommandConfiguration config)
        {
            _adap = adap;
            _command = command;
            _config = config;
        }

        public ExecuteNonQueryResult ExecuteNonQuery()
        {
            _config.Configure(_command);
            return new ExecuteNonQueryResult(_adap);
        }

        public ExecuteReaderResult ExecuteReader()
        {
            _config.Configure(_command);
            return new ExecuteReaderResult(_adap);
        }

        public ExecuteScalarResult<T> ExecuteScalar<T>()
        {
            _config.Configure(_command);
            return new ExecuteScalarResult<T>(_adap);
        }
    }

    public static class DbTypeUtil
    {
        private static readonly IDictionary<Type, DbType> _typeMap = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            [typeof(System.Data.Linq.Binary)] = DbType.Binary
        };

        /// <summary>
        /// Converts a SqlDbType to a DbType.
        /// </summary>
        public static DbType ConvertToDbType(SqlDbType type)
        {
            var p = new System.Data.SqlClient.SqlParameter { SqlDbType = type };
            return p.DbType;
        }

        public static DbType GetDbType(object value)
        {
            if (value == null)
                return DbType.String;
            else
            {
                var key = value.GetType();

                if (key == typeof(DBNull))
                    return DbType.String;

                if (_typeMap.ContainsKey(key))
                    return _typeMap[key];
                else
                    throw new Exception($"Unexpected type: {key}");
            }
        }
    }
}