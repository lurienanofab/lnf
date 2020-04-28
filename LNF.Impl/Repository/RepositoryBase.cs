using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Data;
using NHibernate;
using System.Data;
using System.Linq;

namespace LNF.Impl.Repository
{
    public abstract class RepositoryBase
    {
        private ISessionManager _mgr;
        public ISession Session => _mgr.Session;

        public RepositoryBase(ISessionManager mgr)
        {
            _mgr = mgr;
        }

        protected IQueryable<ActiveLog> ActiveLogQuery(string tableName)
        {
            return Session.Query<ActiveLog>().Where(x => x.TableName == tableName);
        }

        protected T Require<T>(object id) => Session.Require<T>(id);
    }
}
