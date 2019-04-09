using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.Control.Wago;
using LNF.Impl.DataAccess;
using LNF.Impl.DataAccess.Scheduler;
using LNF.Impl.Encryption;
using LNF.Impl.Logging;
using LNF.Impl.ModelFactory;
using LNF.Impl.Serialization;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using LNF.Models.Data;
using LNF.Models.Mail;
using LNF.Models.PhysicalAccess;
using LNF.Models.Scheduler;
using LNF.Models.Worker;
using LNF.Repository;
using LNF.Scheduler;
using OnlineServices.Api.Billing;
using OnlineServices.Api.Data;
using OnlineServices.Api.Mail;
using OnlineServices.Api.PhysicalAccess;
using OnlineServices.Api.Scheduler;
using OnlineServices.Api.Worker;
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

                _.For<IDataRepository>().Use<DataRepository>(); // not a singleton
                _.For<ISchedulerRepository>().Use<SchedulerRepository>(); // not a singleton
                //_.For<IUnitOfWork>().Use<NHibernateUnitOfWork>(); // not a singleton

                //_.For<ISession>().Singleton().Use<NHibernateSession>();
                _.For<IProvider>().Singleton().Use<ServiceProvider>();
                _.For<IDataAccessService>().Singleton().Use<NHibernateDataAccess>();
                _.For<ILogService>().Singleton().Use<ServiceLogService>();
                _.For<IControlService>().Singleton().Use<WagoControlService>();
                _.For<IEncryptionService>().Singleton().Use<EncryptionService>();
                _.For<ISerializationService>().Singleton().Use<SerializationService>();

                _.For<IDataService>().Singleton().Use<DataService>();

                // Mail API
                _.For<IMailService>().Singleton().Use<MailService>();

                // Billing API
                _.For<IBillingService>().Singleton().Use<BillingService>();
                _.For<IAccountSubsidyManager>().Singleton().Use<Billing.AccountSubsidyManager>();
                _.For<IProcessManager>().Singleton().Use<Billing.ProcessManager>();
                _.For<IReportManager>().Singleton().Use<ReportManager>();
                _.For<IToolManager>().Singleton().Use<ToolManager>();
                _.For<Models.Billing.IRoomManager>().Singleton().Use<OnlineServices.Api.Billing.RoomManager>();
                _.For<IStoreManager>().Singleton().Use<StoreManager>();
                _.For<IMiscManager>().Singleton().Use<MiscManager>();

                _.For<IPhysicalAccessService>().Singleton().Use<PhysicalAccessService>();
                _.For<ISchedulerService>().Singleton().Use<SchedulerService>();
                _.For<IWorkerService>().Singleton().Use<WorkerService>();

                _.For<IModelFactory>().Singleton().Use<ValueInjecterModelFactory>();

                _.For<IAccountManager>().Singleton().Use<AccountManager>();
                _.For<IChargeTypeManager>().Singleton().Use<ChargeTypeManager>();
                _.For<IClientManager>().Singleton().Use<ClientManager>();
                _.For<IClientOrgManager>().Singleton().Use<ClientOrgManager>();
                _.For<IClientRemoteManager>().Singleton().Use<ClientRemoteManager>();
                _.For<IOrgManager>().Singleton().Use<OrgManager>();
                _.For<IActiveDataItemManager>().Singleton().Use<ActiveDataItemManager>();
                _.For<ICostManager>().Singleton().Use<CostManager>();
                _.For<IDryBoxManager>().Singleton().Use<DryBoxManager>();
                _.For<Models.Data.IRoomManager>().Singleton().Use<Data.RoomManager>();

                _.For<IBillingTypeManager>().Singleton().Use<BillingTypeManager>();
                _.For<IToolBillingManager>().Singleton().Use<ToolBillingManager>();
                _.For<IApportionmentManager>().Singleton().Use<LNF.Billing.ApportionmentManager>();

                _.For<IReservationManager>().Singleton().Use<ReservationManager>();
                _.For<IReservationInviteeManager>().Singleton().Use<ReservationInviteeManager>();
                _.For<IResourceManager>().Singleton().Use<ResourceManager>();
                _.For<IEmailManager>().Singleton().Use<EmailManager>();
                _.For<IProcessInfoManager>().Singleton().Use<ProcessInfoManager>();

                _.For<IAdministrativeHelper>().Singleton().Use<AdministrativeHelper>();
                _.For<IReadToolDataManager>().Singleton().Use<ReadToolDataManager>();
                _.For<IReadRoomDataManager>().Singleton().Use<ReadRoomDataManager>();
                _.For<IReadStoreDataManager>().Singleton().Use<ReadStoreDataManager>();

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
