using LNF.Repository;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace LNF.CommonTools
{
    public class SQLDBAccess : UnitOfWorkAdapter
    {
        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }

        public static SQLDBAccess Create(string key)
        {
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var conn = factory.CreateConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            return Create(conn);
        }

        public static SQLDBAccess Create(DbConnection conn)
        {
            var result = new SQLDBAccess();
            result.Configure(conn);
            return result;
        }

        private SQLDBAccess() { }

        private void Configure(DbConnection conn)
        {
            Connection = conn;
            Connection.Open();

            Transaction = Connection.BeginTransaction(IsolationLevel.ReadCommitted);

            SelectCommand = CreateCommand();
            InsertCommand = CreateCommand();
            UpdateCommand = CreateCommand();
            InsertCommand = CreateCommand();
            DeleteCommand = CreateCommand();
        }

        private DbCommand CreateCommand()
        {
            DbCommand result = Connection.CreateCommand();
            result.Connection = Connection;
            result.Transaction = Transaction;
            result.CommandText = string.Empty;
            result.CommandType = CommandType.StoredProcedure;
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                Transaction.Commit();
            }
            catch
            {
                Transaction.Rollback();
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed)
                    Connection.Close();

                Transaction.Dispose();
                Connection.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
