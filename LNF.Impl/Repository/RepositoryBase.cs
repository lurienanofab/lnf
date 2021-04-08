using LNF.DataAccess;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Data;
using LNF.Repository;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Impl.Repository
{
    public abstract class RepositoryBase
    {
        protected static MemoryCache Cache { get; } = new MemoryCache("RepositoryCache");

        private ISessionManager _mgr;
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
