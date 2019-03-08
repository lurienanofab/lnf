using LNF.CommonTools;
using LNF.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Repository
{
    public static class DA
    {
        /// <summary>
        /// Gets an ISession instance that uses the current data access context.
        /// </summary>
        public static ISession Current => ServiceProvider.Current.Use<ISession>();

        /// <summary>
        /// Initiates a new current session. All subsequent data access actions (DA.Current) will be in one transaction which is committed when this instance is disposed.
        /// </summary>
        /// <returns></returns>
        public static IDisposable StartUnitOfWork() => ServiceProvider.Current.Use<IUnitOfWork>();

        public static ISchedulerRepository SchedulerRepository => ServiceProvider.Current.Use<ISchedulerRepository>();

        /// <summary>
        /// Create a new DataCommand instance.
        /// </summary>
        /// <param name="type">The CommandType used for selects. Also the default CommandType for updates (can be changed in Update action).</param>
        public static DataCommandBase Command(CommandType type = CommandType.StoredProcedure) => DataCommand.Create(type);


        //The methods that were previously defined here have been moved to LNF.Repository.ISession
        //I think these methods should only be defined once, so now they are accessed via Current
    }

    public class DataCommand : DataCommandBase
    {
        private DataCommand(CommandType type) : base(type) { }

        protected override UnitOfWorkAdapter GetAdapter() => DA.Current.GetAdapter();

        public static DataCommand Create(CommandType type = CommandType.StoredProcedure) => new DataCommand(type);
    }

    public class DefaultDataCommand : DataCommandBase
    {
        private DefaultDataCommand(CommandType type) : base(type) { }

        protected override UnitOfWorkAdapter GetAdapter() => SQLDBAccess.Create("cnSselData");

        public static DefaultDataCommand Create(CommandType type = CommandType.StoredProcedure) => new DefaultDataCommand(type);
    }

    public class ReadOnlyDataCommand : DataCommandBase
    {
        private ReadOnlyDataCommand(CommandType type) : base(type) { }

        protected override UnitOfWorkAdapter GetAdapter() => SQLDBAccess.Create("cnSselDataReadOnly");

        public static ReadOnlyDataCommand Create(CommandType type = CommandType.StoredProcedure) => new ReadOnlyDataCommand(type);
    }

    public abstract class DataCommandBase
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

        protected abstract UnitOfWorkAdapter GetAdapter();
        //{
        //    if (_adap != null) return _adap();
        //    else throw new Exception("Use the static Create method or override this class.");
        //}

        public DataCommandBase Timeout(int value)
        {
            _configs["select"].SetCommandTimeout(value);
            return this;
        }

        public DataCommandBase MapSchema()
        {
            _mapSchema = true;
            return this;
        }

        /// <summary>
        /// Adds parameters to the select command.
        /// </summary>
        public DataCommandBase Param(object parameters)
        {
            _configs["select"].AddParameter(parameters);
            return this;
        }

        /// <summary>
        /// Adds parameters to the select command.
        /// </summary>
        public DataCommandBase Param(IDictionary<string, object> parameters)
        {
            _configs["select"].AddParameter(parameters);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the select command.
        /// </summary>
        public DataCommandBase Param(string name, object value)
        {
            _configs["select"].AddParameter(name, value);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the select command with the specified parameter direction.
        /// </summary>
        public DataCommandBase Param(string name, object value, ParameterDirection direction)
        {
            _configs["select"].AddParameter(name, value, direction);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the select command if the condition is true.
        /// </summary>
        public DataCommandBase Param(string name, bool test, object value)
        {
            _configs["select"].AddParameter(name, test, value);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the select command using v1 if the condition is true, or v2 if the condition is false.
        /// </summary>
        public DataCommandBase Param(string name, bool test, object v1, object v2)
        {
            _configs["select"].AddParameter(name, test, v1, v2);
            return this;
        }

        /// <summary>
        /// Adds parameters to the select command as p1, p2, ...
        /// </summary>
        public DataCommandBase ParamList(string prefix, IEnumerable values)
        {
            _configs["select"].AddParameterList(prefix, values);
            return this;
        }

        public DataTable FillDataTable(string commandText)
        {
            var dt = new DataTable();
            FillDataTable(dt, commandText);
            return dt;
        }

        public void FillDataTable(DataTable dt, string commandText)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                MapSchema(adap, dt);
                adap.Fill(dt);
            }
        }

        public DataSet FillDataSet(string commandText)
        {
            var ds = new DataSet();
            FillDataSet(ds, commandText);
            return ds;
        }

        public void FillDataSet(DataSet ds, string commandText)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                MapSchema(adap, ds);
                adap.Fill(ds);
            }
        }

        public void FillDataSet(DataSet ds, string commandText, string srcTable)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                MapSchema(adap, ds);
                adap.Fill(ds, srcTable);
            }
        }

        public IDataReader ExecuteReader(string commandText)
        {
            var adap = GetSelectAdapter(commandText);
            return adap.SelectCommand.ExecuteReader();
        }

        public T ExecuteScalar<T>(string commandText)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                return Utility.ConvertTo(adap.SelectCommand.ExecuteScalar(), default(T));
            }
        }

        public ExecuteNonQueryResult ExecuteNonQuery(string commandText)
        {
            using (var adap = GetSelectAdapter(commandText))
            {
                int value = adap.SelectCommand.ExecuteNonQuery();
                return new ExecuteNonQueryResult(_configs["select"], value);
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

        private UnitOfWorkAdapter GetSelectAdapter(string commandText)
        {
            var adap = GetAdapter();
            adap.MapTableSchema = _mapSchema;
            _configs["select"].SetCommandText(commandText);
            _configs["select"].Configure(adap.SelectCommand);
            return adap;
        }

        private UnitOfWorkAdapter GetUpdateAdapter(Action<UpdateConfiguration> action)
        {
            var adap = GetAdapter();

            adap.MapTableSchema = _mapSchema;

            action(new UpdateConfiguration(_configs["insert"], _configs["update"], _configs["delete"]));

            _configs["insert"].Configure(adap.InsertCommand);
            _configs["update"].Configure(adap.UpdateCommand);
            _configs["delete"].Configure(adap.DeleteCommand);

            return adap;
        }

        private void MapSchema(UnitOfWorkAdapter adap, DataTable dt)
        {
            if (adap.MapTableSchema)
                adap.FillSchema(dt, SchemaType.Source);
        }

        private void MapSchema(UnitOfWorkAdapter adap, DataSet ds)
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
        private IDbCommand _command = null;
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

        public T GetParamValue<T>(string name, T defval = default(T))
        {
            if (!_command.Parameters.Contains(name))
                return defval;

            var p = (IDbDataParameter)_command.Parameters[name];

            if (p == null)
                return defval;

            return Utility.ConvertTo(p.Value, defval);
        }

        internal void Configure(IDbCommand command)
        {
            _command = command;

            _command.CommandType = _commandType;
            _command.CommandText = FormatCommandText();
            _command.CommandTimeout = _commandTimeout;

            _command.Parameters.Clear();

            foreach (var kvp in _parameters)
            {
                var def = kvp.Value;

                var p = _command.CreateParameter();

                p.ParameterName = def.Name;
                p.Value = def.Value;
                p.Direction = def.Direction;
                p.DbType = def.DbType;
                p.Size = def.Size;
                p.SourceColumn = string.IsNullOrEmpty(def.SourceColumn) ? def.Name.TrimStart('@') : def.SourceColumn;

                _command.Parameters.Add(p);
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

    public class ExecuteNonQueryResult
    {
        private readonly CommandConfiguration _config;

        internal ExecuteNonQueryResult(CommandConfiguration config, int value)
        {
            _config = config;
            Value = value;
        }

        public int Value { get; }

        public T GetParamValue<T>(string name, T defval = default(T)) => _config.GetParamValue(name, defval);
    }

    public class BatchConfiguration
    {
        public BatchCommandConfiguration Select { get; }
        public BatchCommandConfiguration Insert { get; }
        public BatchCommandConfiguration Update { get; }
        public BatchCommandConfiguration Delete { get; }

        internal BatchConfiguration(IDbDataAdapter adap, IDictionary<string, CommandConfiguration> configs)
        {
            Select = new BatchCommandConfiguration(adap, adap.SelectCommand, configs["select"]);
            Insert = new BatchCommandConfiguration(adap, adap.InsertCommand, configs["insert"]);
            Update = new BatchCommandConfiguration(adap, adap.UpdateCommand, configs["update"]);
            Delete = new BatchCommandConfiguration(adap, adap.DeleteCommand, configs["delete"]);
        }
    }

    public class BatchCommandConfiguration : CommandConfiguration
    {
        private readonly IDbDataAdapter _adap;
        private readonly IDbCommand _command;
        private readonly CommandConfiguration _config;

        internal BatchCommandConfiguration(IDbDataAdapter adap, IDbCommand command, CommandConfiguration config)
        {
            _adap = adap;
            _command = command;
            _config = config;
        }

        public ExecuteNonQueryResult ExecuteNonQuery()
        {
            _config.Configure(_command);
            var value = _command.ExecuteNonQuery();
            return new ExecuteNonQueryResult(_config, value);
        }

        public IDataReader ExecuteReader()
        {
            _config.Configure(_command);
            return _command.ExecuteReader();
        }

        public T ExecuteScalar<T>()
        {
            _config.Configure(_command);
            return Utility.ConvertTo(_command.ExecuteScalar(), default(T));
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