using LNF.Impl.DataAccess.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using NHibernate.Context;
using System;

namespace LNF.Impl
{
    public abstract class NHibernateDataAccess<TContext> : IDataAccessProvider where TContext : ICurrentSessionContext
    {
        private readonly NHibernateRepository<TContext> _Repository;
        private readonly string _universalPassword;

        protected NHibernateDataAccess()
        {
            _Repository = new NHibernateRepository<TContext>();
            _universalPassword = Providers.GetConfigurationSection().DataAccess.UniversalPassword;
        }

        public IUnitOfWork StartUnitOfWork()
        {
            return new NHibernateUnitOfWork<TContext>();
        }

        public IRepository Repository
        {
            get { return _Repository; }
        }

        public ISchedulerRepositoryCollection Scheduler
        {
            get { return SchedulerRepositoryCollection<TContext>.Current; }
        }

        public string UniversalPassword
        {
            get { return _universalPassword; }
        }

        public bool ShowSql { get; set; }

        public void Assert()
        {
            //this will ensure the session factory has been created.
            var id = SessionManager<TContext>.Current.GetFactoryID();
            if (id == Guid.Empty)
                throw new Exception("Unable to create the session factory.");
        }
    }

    public class NHibernateWebDataAccess : NHibernateDataAccess<WebSessionContext> { }
    public class NHibernateCallDataAccess : NHibernateDataAccess<CallSessionContext> { }
    public class NHibernateThreadStaticDataAccess : NHibernateDataAccess<ThreadStaticSessionContext> { }
}
