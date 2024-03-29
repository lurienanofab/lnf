﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF
{
    public static class SqlClientExtentions
    {
        /// <summary>
        /// Creates a SqlCommand object with the given connection, sql, and CommandType. Uses the connection's ConnectionTimeout property to set CommandTimeout
        /// </summary>
        public static SqlCommand CreateCommand(this SqlConnection conn, string sql, CommandType commandType = CommandType.StoredProcedure, bool useConnectionTimeout = true)
        {
            // [2022-04-11 jg] Any SqlCommand objects created with this method will use the following value for the minimum timeout value in seconds. This prevents accidently
            //      setting the timeout ridiculously low by not setting the ConnectionTimeout in the web.config ConnectionString. So ConnectionTimeout can be used to extend
            //      the command timeout beyond 60 seconds but never makes it less.
            var minTimeoutSeconds = 60;

            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = commandType;
            if (useConnectionTimeout)
                cmd.CommandTimeout = Math.Max(conn.ConnectionTimeout, minTimeoutSeconds); // enforce the minimum
            return cmd;
        }

        /// <summary>
        /// Creates a SqlCommand object with the given connection, transaction, sql, and CommandType. Uses the connection's ConnectionTimeout property to set CommandTimeout
        /// </summary>
        public static SqlCommand CreateCommand(this SqlConnection conn, SqlTransaction tx, string sql, CommandType commandType = CommandType.StoredProcedure, bool useConnectionTimeout = true)
        {
            var cmd = conn.CreateCommand(sql, commandType, useConnectionTimeout);
            cmd.Transaction = tx;
            return cmd;
        }

        public static void AddWithValue(this SqlParameterCollection parameters, string parameterName, object value, SqlDbType dbType)
        {
            var p = new SqlParameter(parameterName, value) { SqlDbType = dbType };
            parameters.Add(p);
        }

        public static void AddWithValue(this SqlParameterCollection parameters, string parameterName, object value, SqlDbType dbType, int size)
        {
            var p = new SqlParameter(parameterName, value) { SqlDbType = dbType, Size = size };
            parameters.Add(p);
        }

        public static void AddOutputParameter(this SqlParameterCollection parameters, string parameterName, SqlDbType dbType)
        {
            var p = new SqlParameter(parameterName, dbType) { Direction = ParameterDirection.Output };
            parameters.Add(p);
        }

        public static void AddOutputParameter(this SqlParameterCollection parameters, string parameterName, SqlDbType dbType, int size)
        {
            var p = new SqlParameter(parameterName, dbType, size) { Direction = ParameterDirection.Output };
            parameters.Add(p);
        }
    }
}
