using LNF.Billing;
using LNF.CommonTools;
using LNF.Impl.Authorization;
using LNF.Impl.Billing;
using LNF.Impl.Billing.Report;
using LNF.Impl.Control.Wago;
using LNF.Impl.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.DataAccess.Scheduler;
using LNF.Impl.Encryption;
using LNF.Impl.Logging;
using LNF.Impl.Mail;
using LNF.Impl.ModelFactory;
using LNF.Impl.PhysicalAccess;
using LNF.Impl.Scheduler;
using LNF.Impl.Serialization;
using LNF.Impl.Worker;
using LNF.Models.Authorization;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using LNF.Models.Data;
using LNF.Models.Data.Utility;
using LNF.Models.Mail;
using LNF.Models.PhysicalAccess;
using LNF.Models.Scheduler;
using LNF.Models.Worker;
using LNF.Repository;
using LNF.Scheduler;
using StructureMap;
using System;

namespace LNF.Impl.DependencyInjection
{
    public class StructureMapResolver : IDependencyResolver
    {
        private Container _container;

        public StructureMapResolver(Registry registry)
        {
            _container = new Container(_ =>
            {
                _.AddRegistry(registry);

                //_.For<LNF.Data.IDataRepository>().Use<DataRepository>(); // not a singleton
                _.For<ISchedulerRepository>().Use<SchedulerRepository>(); // not a singleton
                //_.For<IUnitOfWork>().Use<NHibernateUnitOfWork>(); // not a singleton

                _.For<ILogService>().Singleton().Use<ServiceLogService>();
                _.For<IControlService>().Singleton().Use<WagoControlService>();
                _.For<IEncryptionService>().Singleton().Use<EncryptionService>();
                _.For<ISerializationService>().Singleton().Use<SerializationService>();

                _.For<IModelFactory>().Singleton().Use<ValueInjecterModelFactory>();

                _.For<IPhysicalAccessService>().Singleton().Use<PhysicalAccessService>();
                _.For<IWorkerService>().Singleton().Use<WorkerService>();
                _.For<IMailService>().Singleton().Use<MailService>();
                
                // Billing API
                _.For<IBillingServices>().Singleton().Use<BillingService>();
                _.For<IAccountSubsidyManager>().Singleton().Use<AccountSubsidyManager>();
                _.For<IProcessManager>().Singleton().Use<ProcessManager>();
                _.For<IReportManager>().Singleton().Use<ReportManager>();
                _.For<IToolBillingManager>().Singleton().Use<ToolBillingManager>();
                _.For<IRoomBillingManager>().Singleton().Use<RoomBillingManager>();
                _.For<IStoreBillingManager>().Singleton().Use<StoreBillingManager>();
                _.For<IMiscBillingManager>().Singleton().Use<MiscBillingManager>();
                _.For<IBillingTypeManager>().Singleton().Use<BillingTypeManager>();
                _.For<IApportionmentManager>().Singleton().Use<ApportionmentManager>();

                // Scheduler API
                _.For<ISchedulerService>().Singleton().Use<SchedulerService>();
                _.For<IProcessInfoManager>().Singleton().Use<ProcessInfoManager>();
                _.For<IReservationManager>().Singleton().Use<ReservationManager>();
                _.For<IResourceManager>().Singleton().Use<ResourceManager>();

                // Data API
                _.For<IDataService>().Singleton().Use<DataService>();
                _.For<IAccountManager>().Singleton().Use<AccountManager>();
                _.For<IClientManager>().Singleton().Use<ClientManager>();
                _.For<IOrgManager>().Singleton().Use<OrgManager>();
                _.For<IActiveLogManager>().Singleton().Use<ActiveLogManager>();
                _.For<ICostManager>().Singleton().Use<CostManager>();
                _.For<IDryBoxManager>().Singleton().Use<DryBoxManager>();
                _.For<IRoomManager>().Singleton().Use<RoomManager>();
                _.For<IServiceLogManager>().Singleton().Use<ServiceLogManager>();
                _.For<IUtilityManager>().Singleton().Use<UtilityManager>();
                _.For<IFeedManager>().Singleton().Use<FeedManager>();

                _.For<IEmailManager>().Singleton().Use<EmailManager>();

                _.For<IAdministrativeHelper>().Singleton().Use<AdministrativeHelper>();
                _.For<IReadToolDataManager>().Singleton().Use<ReadToolDataManager>();
                _.For<IReadRoomDataManager>().Singleton().Use<ReadRoomDataManager>();
                _.For<IReadStoreDataManager>().Singleton().Use<ReadStoreDataManager>();

                _.For<IAuthorizationService>().Singleton().Use<AuthorizationService>();

                _.For<IDependencyResolver>().Singleton().Use(this);

                _.For<IProvider>().Singleton().Use<ServiceProvider>();
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
