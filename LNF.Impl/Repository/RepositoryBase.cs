using LNF.DataAccess;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Data;
using LNF.Repository;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Impl.Repository
{
    public abstract class SqlClientRepositoryBase
    {
        protected SqlConnection NewConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
        }

        protected SqlCommand NewCommand(string sql, CommandType commandType = CommandType.StoredProcedure)
        {
            var conn = NewConnection();
            var result = NewCommand(conn, sql, commandType);
            return result;
        }

        protected SqlCommand NewCommand(SqlConnection conn, string sql, CommandType commandType = CommandType.StoredProcedure)
        {
            return new SqlCommand(sql, conn) { CommandType = commandType };
        }
    }

    public abstract class RepositoryBase
    {
        protected static MemoryCache Cache { get; } = new MemoryCache("RepositoryCache");

        private readonly ISessionManager _mgr;
        public NHibernate.ISession Session => _mgr.Session;

        public RepositoryBase(ISessionManager mgr)
        {
            _mgr = mgr;
        }

        protected IUnitOfWork NewUnitOfWork()
        {
            return new NHibernateUnitOfWork(_mgr);
        }

        protected IQueryable<ActiveLog> ActiveLogQuery(string tableName)
        {
            return Session.Query<ActiveLog>().Where(x => x.TableName == tableName);
        }

        protected T Require<T>(object id) => Session.Require<T>(id);

        protected IDataCommand DataCommand(CommandType type = CommandType.StoredProcedure)
        {
            return LNF.Repository.DataCommand.Create(type);
        }
    }
}
