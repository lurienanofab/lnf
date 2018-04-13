using LNF.Data;
using LNF.Impl.DependencyInjection;
using LNF.Repository;
using LNF.Scheduler;
using System;

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

        public IDataRepository DataRepository => DependencyResolver.GetInstance<IDataRepository>();

        public ISchedulerRepository SchedulerRepository => DependencyResolver.GetInstance<ISchedulerRepository>();

        public string UniversalPassword => _config.UniversalPassword;

        public bool ShowSql => _config.ShowSql;
    }
}
