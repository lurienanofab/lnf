using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LNF.Repository
{
    public static class DataItemExtensions
    {
        public static string ToJson(this IDataItem item)
        {
            return Providers.Serialization.Json.SerializeObject(item);
        }
    }

    /// <summary>
    /// Provides helper methods for UnitOfWorkAdapter
    /// </summary>
    public static class UnitOfWorkAdapterExtenstions
    {
        /// <summary>
        /// Sets the SelectCommand CommandType property
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <param name="value">The CommandType value to set</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter CommandType(this UnitOfWorkAdapter adap, CommandType value)
        {
            adap.SelectCommand.CommandType = value;
            return adap;
        }

        /// <summary>
        /// Gets the SelectCommand CommandType property
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static CommandType CommandType(this UnitOfWorkAdapter adap)
        {
            return adap.SelectCommand.CommandType;
        }

        /// <summary>
        /// Sets the SelectCommand CommandType property to CommandType.StoredProcedure (this is the default value)
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter CommandTypeStoredProcedure(this UnitOfWorkAdapter adap)
        {
            adap.CommandType(System.Data.CommandType.StoredProcedure);
            return adap;
        }

        /// <summary>
        /// Sets the SelectCommand CommandType property to CommandType.Text
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter CommandTypeText(this UnitOfWorkAdapter adap)
        {
            adap.CommandType(System.Data.CommandType.Text);
            return adap;
        }

        /// <summary>
        /// Sets the SelectCommand CommandType property to CommandType.TableDirect
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter CommandTypeTableDirect(this UnitOfWorkAdapter adap)
        {
            adap.CommandType(System.Data.CommandType.TableDirect);
            return adap;
        }

        /// <summary>
        /// Sets the SelectCommand parameters using reflection - deletes existing parameters first
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <param name="queryParams">An object used to get query parameters by reflecting property names and values</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter ApplyParameters(this UnitOfWorkAdapter adap, object queryParams)
        {
            adap.SelectCommand.ApplyParameters(queryParams);
            return adap;
        }

        /// <summary>
        /// Sets the SelectCommand parameters using key/value pairs - deletes existing parameters first
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <param name="queryParams">A dictionary containg key values pairs for parameter names and values</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter ApplyParameters(this UnitOfWorkAdapter adap, IEnumerable<KeyValuePair<string, object>> queryParams)
        {
            adap.SelectCommand.ApplyParameters(queryParams);
            return adap;
        }

        /// <summary>
        /// Gets a paramter value from the SelectCommand and converts it to an expected Type
        /// </summary>
        /// <typeparam name="T">The expected Type of the parameter</typeparam>
        /// <param name="adap">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <returns>A value of Type T (if conversion fails for any reason the default value of Type T is returned)</returns>
        public static T GetParameterValue<T>(this UnitOfWorkAdapter adap, string name)
        {
            return adap.SelectCommand.GetParameterValue<T>(name);
        }

        /// <summary>
        /// Adds an item to the SelectCommand parameter collection
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter AddParameter(this UnitOfWorkAdapter adap, string name, object value)
        {
            adap.SelectCommand.AddParameter(name, value);
            return adap;
        }

        /// <summary>
        /// Adds an item to the SelectCommand parameter collection if the condition is true
        /// </summary>
        /// <param name="adap">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="condition">A value that must be true in order for the parameter to be added</param>
        /// <param name="value">The parameter value</param>
        /// <returns>The current UnitOfWorkAdapter object</returns>
        public static UnitOfWorkAdapter AddParameterIf(this UnitOfWorkAdapter adap, string name, bool condition, object value)
        {
            adap.SelectCommand.AddParameterIf(name, condition, value);
            return adap;
        }
    }

    /// <summary>
    /// Providers helper methods for System.Data.Common.DbCommand
    /// </summary>
    public static class DbCommandExtensions
    {
        /// <summary>
        /// Sets the CommandType property
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="value">The CommandType to set</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand CommandType(this DbCommand cmd, CommandType value)
        {
            cmd.CommandType = value;
            return cmd;
        }

        /// <summary>
        /// Gets the CommandType property.
        /// </summary>
        public static CommandType CommandType(this DbCommand cmd)
        {
            return cmd.CommandType;
        }

        /// <summary>
        /// Sets the CommandType property to CommandType.StoredProcedure (this is the default value).
        /// </summary>
        public static DbCommand CommandTypeStoredProcedure(this DbCommand cmd)
        {
            cmd.CommandType(System.Data.CommandType.StoredProcedure);
            return cmd;
        }

        /// <summary>
        /// Sets the CommandType property to CommandType.Text.
        /// </summary>
        /// <param name="cmd">The object on which the action is performed</param>
        /// <returns>The current DbComand object</returns>
        public static DbCommand CommandTypeText(this DbCommand cmd)
        {
            cmd.CommandType(System.Data.CommandType.Text);
            return cmd;
        }

        /// <summary>
        /// Sets the CommandType property to CommandType.TableDirect
        /// </summary>
        /// <param name="cmd">The object on which the action is performed</param>
        /// <returns>The current DbComand object</returns>
        public static DbCommand CommandTypeTableDirect(this DbCommand cmd)
        {
            cmd.CommandType(System.Data.CommandType.TableDirect);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which the action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The paramter value</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            if (value == null)
                p.Value = DBNull.Value;
            else
                p.Value = value;
            cmd.Parameters.Add(p);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        /// <param name="direction">The parameter direction - ParameterDirection.Output will cause the parameter value to be set by the database when the statement is executed</param>
        /// <returns></returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, object value, ParameterDirection direction)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            p.Direction = direction;
            cmd.Parameters.Add(p);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter DbType value</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, DbType type)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.SourceColumn = name.TrimStart('@');
            cmd.Parameters.Add(p);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter SqlDbType value</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, SqlDbType type)
        {
            return AddParameter(cmd, name, type.GetDbType());
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter DbType value</param>
        /// <param name="size">The parameter size</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, DbType type, int size)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.SourceColumn = name.TrimStart('@');
            p.Size = size;
            cmd.Parameters.Add(p);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter SqlDbType value</param>
        /// <param name="size">The parameter size</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, SqlDbType type, int size)
        {
            return AddParameter(cmd, name, type.GetDbType(), size);
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter DbType value</param>
        /// <param name="direction">The parameter direction - ParameterDirection.Output will cause the parameter value to be set by the database when the statement is executed</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, DbType type, ParameterDirection direction)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.SourceColumn = name.TrimStart('@');
            p.Direction = direction;
            cmd.Parameters.Add(p);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter SqlDbType value</param>
        /// <param name="direction">The parameter direction - ParameterDirection.Output will cause the parameter value to be set by the database when the statement is executed</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, SqlDbType type, ParameterDirection direction)
        {
            return AddParameter(cmd, name, type.GetDbType(), direction);
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter DbType value</param>
        /// <param name="size">The parameter size</param>
        /// <param name="direction">The parameter direction - ParameterDirection.Output will cause the parameter value to be set by the database when the statement is executed</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, DbType type, int size, ParameterDirection direction)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.SourceColumn = name.TrimStart('@');
            p.Size = size;
            p.Direction = direction;
            cmd.Parameters.Add(p);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="type">The paramter SqlDbType value</param>
        /// <param name="size">The parameter size</param>
        /// <param name="direction">The parameter direction - ParameterDirection.Output will cause the parameter value to be set by the database when the statement is executed</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameter(this DbCommand cmd, string name, SqlDbType type, int size, ParameterDirection direction)
        {
            return AddParameter(cmd, name, type.GetDbType(), size, direction);
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection if the condition is true 
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="condition">A value that must be true in order for the parameter to be added</param>
        /// <param name="value">The parameter value</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameterIf(this DbCommand cmd, string name, bool condition, object value)
        {
            if (condition)
                AddParameter(cmd, name, value);
            return cmd;
        }

        /// <summary>
        /// Adds an item to the DbCommand DbParameterCollection using one value when the condition is true and another when false
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="condition">Determines which parameter value is used</param>
        /// <param name="value">The parameter value for a true condition</param>
        /// <param name="falseValue">The parameter value for a false condition</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand AddParameterIf(this DbCommand cmd, string name, bool condition, object value, object falseValue)
        {
            return AddParameter(cmd, name, condition ? value : falseValue);
        }

        /// <summary>
        /// Sets parameters using reflection - deletes existing parameters first
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="queryParams">An object used to get query parameters by reflecting property names and values</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand ApplyParameters(this DbCommand cmd, object queryParams)
        {
            var dict = queryParams.GetType().GetProperties().ToDictionary(k => k.Name, v => v.GetValue(queryParams, null));
            return ApplyParameters(cmd, dict);
        }

        /// <summary>
        /// Sets parameters using key/value pairs - deletes existing parameters first
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="queryParams">A dictionary containg key values pairs for parameter names and values</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand ApplyParameters(this DbCommand cmd, IEnumerable<KeyValuePair<string, object>> queryParams)
        {
            if (queryParams == null) return cmd;

            cmd.Parameters.Clear();

            foreach (var kvp in queryParams)
            {
                var name = kvp.Key;
                var value = kvp.Value;
                AddParameter(cmd, name, value);
            }

            return cmd;
        }

        /// <summary>
        /// Executes a statement that returns a single value
        /// </summary>
        /// <typeparam name="T">The expected Type of the return value</typeparam>
        /// <param name="cmd">The object on which this action is perfomed</param>
        /// <returns>A value of Type T (if conversion fails for any reason the default value of Type T is returned)</returns>
        public static T ExecuteScalar<T>(this DbCommand cmd)
        {
            return RepositoryUtility.ConvertTo<T>(cmd.ExecuteScalar(), default(T));
        }

        /// <summary>
        /// Sets the CommandText property and executes a statement that returns a single value
        /// </summary>
        /// <typeparam name="T">The expected Type of the return value</typeparam>
        /// <param name="cmd">The object on which this action is perfomed</param>
        /// <param name="sql">The sql statement</param>
        /// <returns>A value of Type T</returns>
        public static T ExecuteScalar<T>(this DbCommand cmd, string sql)
        {
            cmd.CommandText = sql;
            return ExecuteScalar<T>(cmd);
        }

        /// <summary>
        /// Sets the CommandText property and executes a statement that does not return any rows
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="sql">A statement that does not return any rows</param>
        /// <returns>The number of affected rows</returns>
        public static int ExecuteNonQuery(this DbCommand cmd, string sql)
        {
            cmd.CommandText = sql;
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Sets the CommandText property and creates an IDataReader
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="sql">The select statement</param>
        /// <returns>The current DbCommand object</returns>
        public static IDataReader ExecuteReader(this DbCommand cmd, string sql)
        {
            cmd.CommandText = sql;
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// Removes all parameters for the DbCommand object
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand ClearParameters(this DbCommand cmd)
        {
            cmd.Parameters.Clear();
            return cmd;
        }

        /// <summary>
        /// Gets a paramter value and converts it to an expected Type
        /// </summary>
        /// <typeparam name="T">The expected Type of the parameter</typeparam>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <returns>A value of Type T (if conversion fails for any reason the default value of Type T is returned)</returns>
        public static T GetParameterValue<T>(this DbCommand cmd, string name)
        {
            object obj = cmd.Parameters[name].Value;
            return RepositoryUtility.ConvertTo<T>(obj, default(T));
        }

        /// <summary>
        /// Sets the value of the specified parameter
        /// </summary>
        /// <param name="cmd">The object on which this action is performed</param>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The paramter value to set</param>
        /// <returns>The current DbCommand object</returns>
        public static DbCommand SetParameterValue(this DbCommand cmd, string name, object value)
        {
            cmd.Parameters[name].Value = value;
            return cmd;
        }

        /// <summary>
        /// Converts a SqlDbType to a DbType
        /// </summary>
        /// <param name="type">The SqlDbType value on which this action is performed</param>
        /// <returns>A DbType value equivalent to the specified SqlDbType</returns>
        public static DbType GetDbType(this SqlDbType type)
        {
            var p = new System.Data.SqlClient.SqlParameter();
            p.SqlDbType = type;
            return p.DbType;
        }
    }

    /// <summary>
    /// Provides helper methods for System.Data.IDataReader
    /// </summary>
    public static class IDataReaderExtensions
    {
        /// <summary>
        /// Get the converted value from a IDataReader
        /// </summary>
        /// <typeparam name="T">The expected Type of the return value</typeparam>
        /// <param name="reader">The object on which this action is perfored</param>
        /// <param name="name">The column name</param>
        /// <param name="defval">The default value to use if the value cannot be converted for any reason</param>
        /// <returns>A value of type T</returns>
        public static T Value<T>(this IDataReader reader, string name, T defval)
        {
            try
            {
                return RepositoryUtility.ConvertTo<T>(reader[name], default(T));
            }
            catch
            {
                //the column does not exist, using a try/catch for this sucks but I haven't found a better approach for IDataReader
                return defval;
            }
        }
    }
}
