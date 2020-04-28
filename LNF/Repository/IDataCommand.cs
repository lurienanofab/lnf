using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace LNF.Repository
{
    public interface IDataCommand
    {
        void Batch(Action<BatchConfiguration> action);
        T Batch<T>(Func<BatchConfiguration, T> fn);
        ExecuteNonQueryResult ExecuteNonQuery(string commandText);
        ExecuteReaderResult ExecuteReader(string commandText);
        ExecuteScalarResult<T> ExecuteScalar<T>(string commandText);
        void FillDataSet(DataSet ds, string commandText);
        void FillDataSet(DataSet ds, string commandText, string srcTable);
        DataSet FillDataSet(string commandText);
        void FillDataTable(DataTable dt, string commandText);
        DataTable FillDataTable(string commandText);
        ExecuteFillDataSetResult GetDataSetResult(DataSet ds, string commandText, string srcTable = null);
        ExecuteFillDataSetResult GetDataSetResult(string commandText, string srcTable = null);
        ExecuteFillDataTableResult GetDataTableResult(DataTable dt, string commandText);
        ExecuteFillDataTableResult GetDataTableResult(string commandText);
        IDataCommand MapSchema();

        /// <summary>
        /// Adds parameters to the select command.
        /// </summary>
        IDataCommand Param(object parameters);

        /// <summary>
        /// Adds parameters to the select command.
        /// </summary>
        IDataCommand Param(IDictionary<string, object> parameters);

        /// <summary>
        /// Adds a parameter to the select command.
        /// </summary>
        IDataCommand Param(string name, object value);

        /// <summary>
        /// Adds a parameter to the select command with the specified parameter direction.
        /// </summary>
        IDataCommand Param(string name, object value, ParameterDirection direction);

        /// <summary>
        /// Adds a parameter to the select command if the condition is true.
        /// </summary>
        IDataCommand Param(string name, bool test, object value);

        /// <summary>
        /// Adds a parameter to the select command using v1 if the condition is true, or v2 if the condition is false.
        /// </summary>
        IDataCommand Param(string name, bool test, object v1, object v2);

        /// <summary>
        /// Adds parameters to the select command as IN (p1, p2, ...)
        /// </summary>
        IDataCommand ParamList(string prefix, IEnumerable values);

        IDataCommand Timeout(int value);
        int Update(DataRow[] dataRows, Action<UpdateConfiguration> action);
        int Update(DataSet ds, Action<UpdateConfiguration> action);
        int Update(DataSet ds, string srcTable, Action<UpdateConfiguration> action);
        int Update(DataTable dt, Action<UpdateConfiguration> action);
    }
}