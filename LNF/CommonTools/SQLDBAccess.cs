using LNF.Repository;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace LNF.CommonTools
{
    public class SQLDBAccess : UnitOfWorkAdapter
    {
        private DbProviderFactory _factory;

        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }

        public static SQLDBAccess Create(string key)
        {
            var result = new SQLDBAccess();
            result.Configure(key);
            return result;
        }

        private SQLDBAccess() { }

        private void Configure(string key)
        {
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            Connection = _factory.CreateConnection();
            Connection.ConnectionString = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            Connection.Open();

            Transaction = Connection.BeginTransaction();

            SelectCommand = CreateCommand();
            InsertCommand = CreateCommand();
            UpdateCommand = CreateCommand();
            InsertCommand = CreateCommand();
            DeleteCommand = CreateCommand();
        }

        private DbCommand CreateCommand()
        {
            DbCommand result = _factory.CreateCommand();
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
