using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Repository
{
    //public class SqlDataCommand : DataCommand
    //{
    //    private SqlDataCommand(CommandType type) : base(type) { }

    //    public static SqlDataCommand Create(CommandType type = CommandType.Text) => new SqlDataCommand(type);

    //    protected override UnitOfWorkAdapter GetAdapter()
    //    {
    //        var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
    //        var adap = new SqlUnitOfWorkAdapter(conn);
    //        return adap;
    //    }
    //}

    public class SqlUnitOfWorkAdapter : UnitOfWorkAdapter
    {
        public SqlConnection Connection { get; }
        public SqlTransaction Transaction { get; }

        public SqlUnitOfWorkAdapter(SqlConnection conn)
        {
            var tx = conn.BeginTransaction();

            Connection = conn;
            Transaction = tx;

            SelectCommand = conn.CreateCommand();
            SelectCommand.Transaction = tx;

            InsertCommand = conn.CreateCommand();
            InsertCommand.Transaction = tx;

            UpdateCommand = conn.CreateCommand();
            UpdateCommand.Transaction = tx;

            DeleteCommand = conn.CreateCommand();
            DeleteCommand.Transaction = tx;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Transaction.Commit();
                Connection.Close();
                Transaction.Dispose();
                Connection.Dispose();
            }
        }
    }
}
