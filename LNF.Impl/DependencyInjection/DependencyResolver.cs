using LNF.Data;
using LNF.Impl.Control.Wago;
using LNF.Impl.DataAccess;
using LNF.Impl.DataAccess.Data;
using LNF.Impl.DataAccess.Scheduler;
using LNF.Impl.Email;
using LNF.Impl.Encryption;
using LNF.Impl.Logging;
using LNF.Impl.ModelFactory;
using LNF.Impl.PhysicalAccess;
using LNF.Impl.Serialization;
using LNF.Repository;
using LNF.Scheduler;
using LNF.Scripting;
using StructureMap;

namespace LNF.Impl.DependencyInjection
{
    public class DependencyResolver : IDependencyResolver
    {
        private Container _container;

        public DependencyResolver(Registry registry)
        {
            _container = new Container(_ =>
            {
                _.AddRegistry(registry);

                _.For<IDataRepository>().Singleton().Use<DataRepository>();
                _.For<ISchedulerRepository>().Singleton().Use<SchedulerRepository>();
                _.For<IUnitOfWork>().Use<NHibernateUnitOfWork>(); // not a singleton

                _.For<ISession>().Singleton().Use<NHibernateSession>();
                _.For<IDataAccessService>().Singleton().Use<NHibernateDataAccessService>();
                _.For<ILogService>().Singleton().Use<ServiceLogService>();
                _.For<IEmailService>().Singleton().Use<EmailService>();
                _.For<IControlService>().Singleton().Use<WagoControlService>();
                _.For<IEncryptionService>().Singleton().Use<EncryptionService>();
                _.For<ISerializationService>().Singleton().Use<SerializationService>();
                _.For<IPhysicalAccessService>().Singleton().Use<ProwatchPhysicalAccessService>();
                _.For<IModelFactory>().Singleton().Use<ValueInjecterModelFactory>();

                _.For<IClientManager>().Singleton().Use<ClientManager>();

                _.For<IDependencyResolver>().Singleton().Use(this);
            });
        }

        public T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}
