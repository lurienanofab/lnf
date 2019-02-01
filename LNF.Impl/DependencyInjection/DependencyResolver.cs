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
using LNF.Models;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using LNF.Models.Mail;
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
    public class DependencyResolver : IDependencyResolver
    {
        private Container _container;

        public DependencyResolver(Registry registry)
        {
            _container = new Container(_ =>
            {
                _.AddRegistry(registry);

                //_.For<NHibernate.ISession>().Use(x => x.GetInstance<ISessionManager>().Session);

                _.For<IDataRepository>().Use<DataRepository>(); // not a singleton
                _.For<ISchedulerRepository>().Use<SchedulerRepository>(); // not a singleton
                _.For<IUnitOfWork>().Use<NHibernateUnitOfWork>(); // not a singleton

                _.For<ISession>().Singleton().Use<NHibernateSession>();
                _.For<IDataAccessService>().Singleton().Use<NHibernateDataAccessService>();
                _.For<ILogService>().Singleton().Use<ServiceLogService>();
                _.For<IControlService>().Singleton().Use<WagoControlService>();
                _.For<IEncryptionService>().Singleton().Use<EncryptionService>();
                _.For<ISerializationService>().Singleton().Use<SerializationService>();

                _.For<IDataService>().Singleton().Use<DataClient>();

                // Mail API
                _.For<IMailApi>().Singleton().Use<MailApi>();

                // Billing API
                _.For<IBillingApi>().Singleton().Use<BillingApi>();
                _.For<IAccountSubsidyClient>().Singleton().Use<AccountSubsidyClient>();
                _.For<IProcessClient>().Singleton().Use<ProcessClient>();
                _.For<IReportClient>().Singleton().Use<ReportClient>();
                _.For<IToolClient>().Singleton().Use<ToolClient>();
                _.For<IRoomClient>().Singleton().Use<RoomClient>();
                _.For<IStoreClient>().Singleton().Use<StoreClient>();
                _.For<IMiscClient>().Singleton().Use<MiscClient>();

                _.For<IPhysicalAccessService>().Singleton().Use<PhysicalAccessClient>();
                _.For<ISchedulerService>().Singleton().Use<SchedulerClient>();
                _.For<IWorkerService>().Singleton().Use<WorkerClient>();

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

                _.For<IBillingTypeManager>().Singleton().Use<BillingTypeManager>();
                _.For<IToolBillingManager>().Singleton().Use<ToolBillingManager>();
                _.For<IApportionmentManager>().Singleton().Use<ApportionmentManager>();

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
