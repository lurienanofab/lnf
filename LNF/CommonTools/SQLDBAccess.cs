using LNF.Repository;
using System;
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
            catch (Exception ex1)
            {
                // The Microsoft docs for Rolling backs transactions has a nested try/catch around the rollback call. Without this we can end up with zombie transaction errors.
                // https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqltransaction.rollback?view=dotnet-plat-ext-3.1&viewFallbackFrom=netstandard-2.1

                TransactionCommitException ex = new TransactionCommitException
                {
                    CommitException = ex1,
                    SelectStatement = SelectCommand.CommandText,
                    InsertStatement = InsertCommand.CommandText,
                    DeleteStatement = DeleteCommand.CommandText,
                    UpdateStatement = UpdateCommand.CommandText
                };

                try
                {
                    Transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    ex.RollbackException = ex2;
                }

                if (ConfigurationManager.AppSettings["CommitError.SendEmail"] == "true")
                    SendEmail.Send(0, "SQLDBAccess.Dispose", $"Transaction commit exception [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]", ex.ToString(), SendEmail.SystemEmail, new[] { "lnf-debug@umich.edu" }, isHtml: false);

                if (ConfigurationManager.AppSettings["CommitError.ThrowException"] == "true")
                    throw ex;
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



    public class TransactionCommitException : Exception
    {
        public override string Message => $"Commit:{Environment.NewLine}{GetMessage(CommitException)}{Environment.NewLine}-------------------------{Environment.NewLine}Rollback:{Environment.NewLine}{GetMessage(RollbackException)}";

        public override string StackTrace => $"Commit:{Environment.NewLine}{GetStackTrace(CommitException)}{Environment.NewLine}-------------------------{Environment.NewLine}Rollback:{Environment.NewLine}{GetStackTrace(RollbackException)}";

        public Exception CommitException { get; set; }
        public Exception RollbackException { get; set; }
        public string SelectStatement { get; set; }
        public string InsertStatement { get; set; }
        public string DeleteStatement { get; set; }
        public string UpdateStatement { get; set; }

        public static string GetMessage(Exception ex)
        {
            if (ex == null)
                return string.Empty;
            else
                return ex.Message;
        }

        public static string GetStackTrace(Exception ex)
        {
            if (ex == null)
                return string.Empty;
            else
                return ex.StackTrace;
        }

        public override string ToString()
        {
            var result = base.ToString();

            if (!string.IsNullOrEmpty(SelectStatement))
                result += Environment.NewLine + "--------------------------------------------------" + Environment.NewLine + "select: " + SelectStatement;

            if (!string.IsNullOrEmpty(InsertStatement))
                result += Environment.NewLine + "--------------------------------------------------" + Environment.NewLine + "insert: " + InsertStatement;

            if (!string.IsNullOrEmpty(DeleteStatement))
                result += Environment.NewLine + "--------------------------------------------------" + Environment.NewLine + "delete: " + DeleteStatement;

            if (!string.IsNullOrEmpty(UpdateStatement))
                result += Environment.NewLine + "--------------------------------------------------" + Environment.NewLine + "update: " + UpdateStatement;

            return result;
        }
    }
}


