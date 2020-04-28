using LNF.DataAccess;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LNF.CommonTools
{
    public class DefaultBulkCopy : IBulkCopy
    {
        private SqlConnection _conn;
        private SqlTransaction _trans;
        private SqlBulkCopy _bcp;

        public string DestinationTableName => _bcp.DestinationTableName;

        public DefaultBulkCopy(string destinationTableName)
        {
            _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);

            var dba = SQLDBAccess.Create(_conn);

            if (!(dba.Connection is SqlConnection))
                throw new NotSupportedException("Only SqlConnection type is supported.");

            if (!(dba.Transaction is SqlTransaction))
                throw new NotSupportedException("Only SqlTransaction type is supported.");

            _trans = dba.Transaction as SqlTransaction;

            //SqlBulkCopyOptions options = SqlBulkCopyOptions.TableLock;
            SqlBulkCopyOptions options = SqlBulkCopyOptions.Default;

            _bcp = new SqlBulkCopy(_conn, options, _trans)
            {
                DestinationTableName = destinationTableName,
                BatchSize = 5000
            };
        }

        public void AddColumnMapping(string sourceColumn, string destinationColumn)
        {
            _bcp.ColumnMappings.Add(sourceColumn, destinationColumn);
        }

        public void AddColumnMapping(string columnName)
        {
            AddColumnMapping(columnName, columnName);
        }

        public void WriteToServer(DataTable dt)
        {
            _bcp.WriteToServer(dt);
        }

        public void WriteToServer(DataTable dt, DataRowState state)
        {
            _bcp.WriteToServer(dt, state);
        }

        public void Dispose()
        {
            _trans.Commit();
            _trans.Dispose();
            _conn.Close();
            _conn.Dispose();
            ((IDisposable)_bcp).Dispose();
        }
    }
}
