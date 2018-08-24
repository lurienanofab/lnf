using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.Control.Wago;
using LNF.Impl.DataAccess;
using LNF.Impl.DataAccess.Scheduler;
using LNF.Impl.Email;
using LNF.Impl.Encryption;
using LNF.Impl.Logging;
using LNF.Impl.ModelFactory;
using LNF.Impl.PhysicalAccess;
using LNF.Impl.Serialization;
using LNF.Repository;
using LNF.Scheduler;
using StructureMap;
using System;

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

                _.For<NHibernate.ISession>().Use(x => x.GetInstance<ISessionManager>().Session);

                _.For<IDataRepository>().Use<DataRepository>(); // not a singleton
                _.For<ISchedulerRepository>().Use<SchedulerRepository>(); // not a singleton
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

                _.For<IAccountManager>().Singleton().Use<AccountManager>();
                _.For<IChargeTypeManager>().Singleton().Use<ChargeTypeManager>();
                _.For<IClientManager>().Singleton().Use<ClientManager>();
                _.For<IOrgManager>().Singleton().Use<OrgManager>();
                _.For<IClientOrgManager>().Singleton().Use<ClientOrgManager>();
                _.For<IActiveDataItemManager>().Singleton().Use<ActiveDataItemManager>();
                _.For<ICostManager>().Singleton().Use<CostManager>();
                _.For<IDryBoxManager>().Singleton().Use<DryBoxManager>();

                _.For<IBillingTypeManager>().Singleton().Use<BillingTypeManager>();
                _.For<IToolBillingManager>().Singleton().Use<ToolBillingManager>();

                _.For<IReservationManager>().Singleton().Use<ReservationManager>();
                _.For<IResourceManager>().Singleton().Use<ResourceManager>();
                _.For<IEmailManager>().Singleton().Use<EmailManager>();

                _.For<IReadRoomDataManager>().Singleton().Use<ReadRoomDataManager>();

                _.For<IDependencyResolver>().Singleton().Use(this);
            });
        }

        public T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }

        public void BuildUp(object target)
        {
            _container.BuildUp(target);
        }

        public void Register<TPluginType, TConcreteType>() where TConcreteType : TPluginType
        {
            _container.Configure(c => c.For<TPluginType>().Use<TConcreteType>());
        }

        public void Register(Type pluginType, Type concreteType)
        {
            _container.Configure(c => c.For(pluginType).Use(concreteType));
        }

        public void RegisterSingleton<TPluginType, TConcreteType>() where TConcreteType : TPluginType
        {
            _container.Configure(c => c.For<TPluginType>().Singleton().Use<TConcreteType>());
        }

        public void RegisterSingleton(Type pluginType, Type concreteType)
        {
            _container.Configure(c => c.For(pluginType).Singleton().Use(concreteType));
        }
    }
}
