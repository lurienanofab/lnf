using LNF.Repository;

namespace LNF.Impl.DataAccess
{
    public class NHibernateDataAccessService : IDataAccessService
    {
        private DataAccessServiceElement _config;
        
        protected IDependencyResolver DependencyResolver { get; }

        public NHibernateDataAccessService(IDependencyResolver resolver)
        {
            _config = ServiceProvider.GetConfigurationSection().DataAccess;
            DependencyResolver = resolver;
        }

        public IUnitOfWork StartUnitOfWork()
        {
            return DependencyResolver.GetInstance<IUnitOfWork>();
        }

        public ISession Session => DependencyResolver.GetInstance<ISession>();

        public string UniversalPassword => _config.UniversalPassword;

        public bool ShowSql => _config.ShowSql;
    }
}
