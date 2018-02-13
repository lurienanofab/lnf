using LNF.CommonTools;
using System;
using System.Data;
using System.Data.Common;

namespace LNF.Repository
{
    /// <summary>
    /// A custom DbDataAdapter for working directly with the database server
    /// </summary>
    public abstract class UnitOfWorkAdapter : DbDataAdapter
    {
        /// <summary>
        /// Event raised every time a statement is executed
        /// </summary>
        public event EventHandler<StatementExecutedEventArgs> StatementExecuted;

        /// <summary>
        /// Indicates if the table schema will be mapped when a select statement is executed
        /// </summary>
        public bool MapTableSchema { get; set; }

        /// <summary>
        /// Tell the adapter to retreive primary keys and other table information from the database
        /// </summary>
        /// <returns>The current UnitOfWorkAdapter instance</returns>
        public UnitOfWorkAdapter MapSchema()
        {
            MapTableSchema = true;
            return this;
        }

        /// <summary>
        /// Invoked when a statment is executed
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        protected void OnStatementExecuted(StatementExecutedEventArgs e)
        {
            StatementExecuted?.Invoke(this, e);
        }

        #region FillDataTable
        /// <summary>
        /// Uses the currrent SelectCommand to create a new DataTable
        /// </summary>
        /// <returns>A DataTable object</returns>
        public DataTable FillDataTable()
        {
            DataTable dt = new DataTable();
            FillDataTable(dt);
            return dt;
        }

        /// <summary>
        /// Sets the SelectCommand CommandText property and creates a new DataTable
        /// </summary>
        /// <param name="sql">The select statement</param>
        /// <returns>A DataTable object</returns>
        public DataTable FillDataTable(string sql)
        {
            DataTable dt = new DataTable();
            FillDataTable(dt, sql);
            return dt;
        }

        /// <summary>
        /// Fills a DataTable object with rows from the database
        /// </summary>
        /// <param name="dt">The DataTable to fill</param>
        /// <param name="sql">The select statement</param>
        /// <returns>The number of rows selected</returns>
        public int FillDataTable(DataTable dt, string sql = null)
        {
            SelectCommand.CommandText = sql;

            if (MapTableSchema)
                FillSchema(dt, SchemaType.Mapped);

            int result = Fill(dt);

            OnStatementExecuted(CreateEventArgs("FillDataTable", new[] { sql }, result));

            MapTableSchema = false;

            return result;
        }

        /// <summary>
        /// Updates the database using rows from a DataTable and insert, update, and delete sql statements
        /// </summary>
        /// <param name="dt">The DataTable object</param>
        /// <param name="insertSql">The insert statement</param>
        /// <param name="updateSql">The update statement</param>
        /// <param name="deleteSql">The delete statement</param>
        /// <returns></returns>
        public int UpdateDataTable(DataTable dt, string insertSql = null, string updateSql = null, string deleteSql = null)
        {
            if (!string.IsNullOrEmpty(insertSql))
                InsertCommand.CommandText = insertSql;

            if (!string.IsNullOrEmpty(updateSql))
                UpdateCommand.CommandText = updateSql;

            if (!string.IsNullOrEmpty(deleteSql))
                DeleteCommand.CommandText = deleteSql;

            int result = Update(dt);

            OnStatementExecuted(CreateEventArgs("UpdateDataTable", new[] { insertSql, updateSql, deleteSql }, result));

            return result;
        }
        #endregion

        #region FillDataSet
        /// <summary>
        /// Sets the SelectCommand CommandText property (or uses the current value) and creates a new DataSet
        /// </summary>
        /// <param name="sql">The select statement</param>
        /// <param name="tableName">The table name to use when adding the DataTable to the DataSet</param>
        /// <returns>A DataSet object</returns>
        public DataSet FillDataSet(string sql = null, string tableName = null)
        {
            DataSet ds = new DataSet();
            FillDataSet(ds, sql, tableName);
            return ds;
        }

        /// <summary>
        /// Fills a DataSet object with rows from the database
        /// </summary>
        /// <param name="ds">The DataSet to fill</param>
        /// <param name="sql">The sselect statement</param>
        /// <param name="tableName">The name of the DataTable added to the DataSet</param>
        /// <returns>A DataSet object</returns>
        public int FillDataSet(DataSet ds, string sql = null, string tableName = null)
        {
            if (!string.IsNullOrEmpty(sql))
                SelectCommand.CommandText = sql;

            if (MapTableSchema)
                if (string.IsNullOrEmpty(tableName))
                    FillSchema(ds, SchemaType.Mapped);
                else
                    FillSchema(ds, SchemaType.Mapped, tableName);

            int result;

            if (string.IsNullOrEmpty(tableName))
                result = Fill(ds);
            else
                result = Fill(ds, tableName);

            OnStatementExecuted(CreateEventArgs("FillDataSet", new[] { sql }, result));

            MapTableSchema = false;

            return result;
        }

        /// <summary>
        /// Updates the database using rows from a DataSet and insert, update, and delete sql statements
        /// </summary>
        /// <param name="ds">The DataSet object</param>
        /// <param name="insertSql">The insert statement</param>
        /// <param name="updateSql">The update statement</param>
        /// <param name="deleteSql">The delete statement</param>
        /// <param name="tableName">The name of the DataTable to get rows from</param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet ds, string insertSql = null, string updateSql = null, string deleteSql = null, string tableName = null)
        {
            if (!string.IsNullOrEmpty(insertSql))
                InsertCommand.CommandText = insertSql;

            if (!string.IsNullOrEmpty(updateSql))
                UpdateCommand.CommandText = updateSql;

            if (!string.IsNullOrEmpty(deleteSql))
                DeleteCommand.CommandText = deleteSql;

            int result;

            if (string.IsNullOrEmpty(tableName))
                result = Update(ds);
            else
                result = Update(ds, tableName);

            OnStatementExecuted(CreateEventArgs("UpdateDataSet", new[] { insertSql, updateSql, deleteSql }, result));

            return result;
        }
        #endregion

        #region Default DbCommand Methods
        /// <summary>
        /// Uses the SelectCommand to execute a sql statement that does not return any rows
        /// </summary>
        /// <param name="sql">The sql statement</param>
        /// <returns>The number of rows affected</returns>
        public int ExecuteNonQuery(string sql)
        {
            SelectCommand.CommandText = sql;
            int result = SelectCommand.ExecuteNonQuery();
            OnStatementExecuted(CreateEventArgs("ExecuteNonQuery", new[] { sql }, result));
            return result;
        }

        /// <summary>
        /// Uses the SelectCommand to execute a sql statement to get an IDataReader
        /// </summary>
        /// <param name="sql">The select statement</param>
        /// <returns>An IDataReader instance</returns>
        public IDataReader ExecuteReader(string sql)
        {
            SelectCommand.CommandText = sql;
            IDataReader result = SelectCommand.ExecuteReader();
            OnStatementExecuted(CreateEventArgs("ExecuteReader", new[] { sql }, -1));
            return result;
        }

        /// <summary>
        /// Uses the SelectCommand to execute a sql statement that returns a single value
        /// </summary>
        /// <typeparam name="T">The expected Type of the return value</typeparam>
        /// <param name="sql">The sql statement</param>
        /// <returns>A value of Type T (if conversion fails for any reason the default value of Type T is returned)</returns>
        public T ExecuteScalar<T>(string sql)
        {
            SelectCommand.CommandText = sql;
            object obj = SelectCommand.ExecuteScalar();
            T result = Utility.ConvertTo(obj, default(T));
            OnStatementExecuted(CreateEventArgs("ExecuteScalar", new[] { sql }, -1));
            return result;
        }
        #endregion

        private StatementExecutedEventArgs CreateEventArgs(string method, string[] sql, int count)
        {
            return new StatementExecutedEventArgs(string.Format("LNF.Repository.UnitOfWorkAdapter.{0}", method), sql, count);
        }
    }

    /// <summary>
    /// Arguments for the StatementExecuting event
    /// </summary>
    public class StatementExecutedEventArgs : EventArgs
    {
        /// <summary>
        /// The time at which the statement was exected
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// The method from which the statement was exected
        /// </summary>
        public string From { get; }

        /// <summary>
        /// The sql statement used
        /// </summary>
        public string[] SQL { get; }

        /// <summary>
        /// The number of affected rows
        /// </summary>
        public int Count { get; }

        internal StatementExecutedEventArgs(string from, string[] sql, int count)
        {
            Timestamp = DateTime.Now;
            From = from;
            SQL = sql;
            Count = count;
        }
    }
}
