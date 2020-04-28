using LNF.DataAccess;
using NHibernate.Context;

namespace LNF.Impl.DataAccess
{
    public class NHibernateDataAccess<T> : IDataAccessService where T : ICurrentSessionContext
    {   
        public NHibernateDataAccess(ISessionManager mgr)
        {
            Session = new NHibernateSession(mgr);
        }

        public IUnitOfWork StartUnitOfWork() => new NHibernateUnitOfWork(SessionManager<T>.Current);

        public ISession Session { get; }

        public bool ShowSql => SessionManager<T>.Current.ShowSql;

        public string UniversalPassword => SessionManager<T>.Current.UniversalPassword;

        public bool IsProduction() => SessionManager<T>.Current.IsProduction();
    }
}
