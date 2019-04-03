using LNF.Repository;

namespace LNF.Impl.DataAccess
{
    public class NHibernateDataAccess : IDataAccessService
    {
        private readonly ISessionManager _sessionManager;
        
        public NHibernateDataAccess(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            Session = new NHibernateSession(_sessionManager);
        }

        public IUnitOfWork StartUnitOfWork() => new NHibernateUnitOfWork(_sessionManager);

        public ISession Session { get; }

        public bool ShowSql => _sessionManager.ShowSql;

        public string UniversalPassword => _sessionManager.UniversalPassword;

        public bool IsProduction() => _sessionManager.IsProduction();
    }
}
