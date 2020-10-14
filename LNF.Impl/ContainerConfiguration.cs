using LNF.Authorization;
using LNF.Billing;
using LNF.Billing.Process;
using LNF.Billing.Reports;
using LNF.Cache;
using LNF.Control;
using LNF.Data;
using LNF.DataAccess;
using LNF.Feedback;
using LNF.Impl.Authorization;
using LNF.Impl.Billing;
using LNF.Impl.Billing.Report;
using LNF.Impl.Context;
using LNF.Impl.Control.Wago;
using LNF.Impl.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Feedback;
using LNF.Impl.Inventory;
using LNF.Impl.Logging;
using LNF.Impl.Mail;
using LNF.Impl.Ordering;
using LNF.Impl.PhysicalAccess;
using LNF.Impl.Reporting;
using LNF.Impl.Scheduler;
using LNF.Impl.Scripting;
using LNF.Impl.Store;
using LNF.Impl.Util;
using LNF.Impl.Util.Serialization;
using LNF.Impl.Worker;
using LNF.Inventory;
using LNF.Logging;
using LNF.Mail;
using LNF.Ordering;
using LNF.PhysicalAccess;
using LNF.Reporting;
using LNF.Scheduler;
using LNF.Store;
using LNF.Util;
using LNF.Util.AutoEnd;
using LNF.Util.Encryption;
using LNF.Util.Serialization;
using LNF.Util.SiteMenu;
using LNF.Worker;
using NHibernate.Context;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Diagnostics;
using System;
using System.Linq;
using System.Reflection;

namespace LNF.Impl
{
    public abstract class ContainerConfiguration
    {
        private Container _container;

        public bool SkipDataAccessRegistration { get; set; }

        public ContainerConfiguration(Container container)
        {
            _container = container ?? throw new ArgumentNullException("container");
        }

        public virtual void Configure()
        {
            // Context API
            RegisterContext();

            // DataAccess API
            RegisterSessionManager();

            if (!SkipDataAccessRegistration)
                RegisterDataAccessService();

            RegisterUnitOfWork();

            //Register<IUnitOfWork, NHibernateUnitOfWork>();  // not a singleton

            // Authorization API
            RegisterSingleton<IAuthorizationService, AuthorizationService>();

            // Logging API
            RegisterSingleton<ILoggingService, LoggingService>();

            // Control API
            RegisterSingleton<IControlService, WagoControlService>();

            // Scripting API
            RegisterSingleton<IScriptEngine, DefaultScriptEngine>();

            // Data API
            RegisterSingleton<IDataService, DataService>();
            RegisterSingleton<IAccountRepository, AccountRepository>();
            RegisterSingleton<IClientRepository, ClientRepository>();
            RegisterSingleton<IOrgRepository, OrgRepository>();
            RegisterSingleton<IActiveLogRepository, ActiveLogRepository>();
            RegisterSingleton<ICostRepository, CostRepository>();
            RegisterSingleton<IDryBoxRepository, DryBoxRepository>();
            RegisterSingleton<IRoomRepository, RoomRepository>();
            RegisterSingleton<IServiceLogRepository, ServiceLogRepository>();
            RegisterSingleton<IFeedRepository, FeedRepository>();
            RegisterSingleton<IHolidayRepository, HolidayRepository>();
            RegisterSingleton<IHelpRepository, HelpRepository>();
            RegisterSingleton<IGlobalSettingRepository, GlobalSettingRepository>();
            RegisterSingleton<IMenuRepository, MenuRepository>();

            // Billing API
            RegisterSingleton<IBillingService, BillingService>();
            RegisterSingleton<ISubsidyRepository, SubsidyRepository>();
            RegisterSingleton<IProcessRepository, ProcessRepository>();
            RegisterSingleton<IReportRepository, ReportRepository>();
            RegisterSingleton<IToolBillingRepository, ToolBillingRepository>();
            RegisterSingleton<IRoomBillingRepository, RoomBillingRepository>();
            RegisterSingleton<IStoreBillingRepository, StoreBillingRepository>();
            RegisterSingleton<IMiscBillingRepository, MiscBillingRepository>();
            RegisterSingleton<IBillingTypeRepository, BillingTypeRepository>();
            RegisterSingleton<IApportionmentRepository, ApportionmentRepository>();
            RegisterSingleton<IToolDataRepository, ToolDataRepository>();
            RegisterSingleton<IRoomDataRepository, RoomDataRepository>();
            RegisterSingleton<IStoreDataRepository, StoreDataRepository>();
            RegisterSingleton<IMiscDataRepository, MiscDataRepository>();
            RegisterSingleton<IOrgRechargeRepository, OrgRechargeRepository>();

            // Inventory API
            RegisterSingleton<IInventoryService, InventoryService>();
            RegisterSingleton<IInventoryItemRepository, InventoryItemRepository>();
            RegisterSingleton<ICategoryRepository, CategoryRepository>();

            // Ordering API
            RegisterSingleton<IOrderingService, OrderingService>();
            RegisterSingleton<IPurchaseOrderRepository, PurchaseOrderRepository>();
            RegisterSingleton<IPurchaseOrderItemRepository, PurchaseOrderItemRepository>();
            RegisterSingleton<IPurchaseOrderCategoryRepository, PurchaseOrderCategoryRepository>();
            RegisterSingleton<IVendorRepository, VendorRepository>();
            RegisterSingleton<IApproverRepository, ApproverRepository>();
            RegisterSingleton<IPurchaserRepository, PurchaserRepository>();
            RegisterSingleton<ITrackingRepository, TrackingRepository>();

            // Store API
            RegisterSingleton<IStoreService, StoreService>();

            // Mail API
            RegisterSingleton<IMailService, MailService>();
            RegisterSingleton<IMassEmailRepository, MassEmailRepository>();
            RegisterSingleton<IAttachmentUtility, AttachmentManager>();

            // PhysicalAccess API
            RegisterSingleton<IPhysicalAccessService, PhysicalAccessService>();

            // Scheduler API
            RegisterSingleton<ISchedulerService, SchedulerService>();
            RegisterSingleton<IProcessInfoRepository, ProcessInfoRepository>();
            RegisterSingleton<IReservationRepository, ReservationRepository>();
            RegisterSingleton<IResourceRepository, ResourceRepository>();
            RegisterSingleton<IEmailRepository, EmailRepository>();
            RegisterSingleton<IClientSettingRepository, ClientSettingRepository>();
            RegisterSingleton<IActivityRepository, ActivityRepository>();
            RegisterSingleton<IKioskRepository, KioskRepository>();
            RegisterSingleton<ISchedulerPropertyRepository, SchedulerPropertyRepository>();
            RegisterSingleton<ILabLocationRepository, LabLocationRepository>();

            // Feedback API
            RegisterSingleton<IFeedbackService, FeedbackService>();

            // Worker API
            RegisterSingleton<IWorkerService, WorkerService>();

            // Reporting API
            RegisterSingleton<IReportingService, ReportingService>();
            RegisterSingleton<IClientItemRepository, ClientItemRepository>();
            RegisterSingleton<IClientManagerLogRepository, ClientManagerLogRepository>();
            RegisterSingleton<IClientEmailPreferenceRepository, ClientEmailPreferenceRepository>();
            RegisterSingleton<IManagerUsageChargeRepository, ManagerUsageChargeRepository>();
            RegisterSingleton<IAfterHoursRepository, AfterHoursRepository>();

            // Provider Utility
            RegisterSingleton<IProviderUtility, ProviderUtility>();
            RegisterSingleton<IAutoEndUtility, AutoEndUtility>();
            RegisterSingleton<IEncryptionUtility, EncryptionUtility>();
            RegisterSingleton<ISerializationUtility, SerializationUtility>();
            RegisterSingleton<ISiteMenuUtility, SiteMenuUtility>();

            // Tie it all together
            RegisterSingleton<IProvider, Provider>();
            RegisterSingleton<ICache, DefaultCache>();
        }

        protected virtual void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>(Lifestyle.Transient);
        }

        protected virtual void Register<TService>(Func<TService> instanceCreator)
            where TService : class
        {
            _container.Register(instanceCreator, Lifestyle.Transient);
        }

        protected virtual void Register(Type concreteType, Type implementationType)
        {
            _container.Register(concreteType, implementationType, Lifestyle.Transient);
        }

        protected virtual void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>(Lifestyle.Singleton);
        }

        protected virtual void RegisterSingleton<TService>(Func<TService> instanceCreator)
           where TService : class
        {
            _container.Register(instanceCreator, Lifestyle.Singleton);
        }

        protected virtual void RegisterSingleton(Type concreteType, Type implementationType)
        {
            _container.Register(concreteType, implementationType, Lifestyle.Singleton);
        }

        protected abstract void RegisterContext();

        protected abstract void RegisterSessionManager();

        protected abstract void RegisterDataAccessService();

        protected void RegisterUnitOfWork()
        {
            var registration = Lifestyle.Transient.CreateRegistration(typeof(NHibernateUnitOfWork), _container);
            _container.AddRegistration(typeof(NHibernateUnitOfWork), registration);
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "NHibernateMiddleware creates and disposes IUnitOfWork classes for us.");
        }
    }

    public class WebContainerConfiguration : ContainerConfiguration
    {
        public WebContainerConfiguration(Container container) : base(container) { }

        protected override void RegisterContext() => RegisterSingleton<IContext, WebContext>();
        protected override void RegisterSessionManager() => RegisterSingleton(() => SessionManager<WebSessionContext>.Current);
        protected override void RegisterDataAccessService() => RegisterSingleton<IDataAccessService, NHibernateDataAccess<WebSessionContext>>();
    }

    public class ThreadStaticContainerConfiguration : ContainerConfiguration
    {
        public ThreadStaticContainerConfiguration(Container container) : base(container) { }

        protected override void RegisterContext() => RegisterSingleton<IContext, DefaultContext>();
        protected override void RegisterSessionManager() => RegisterSingleton(() => SessionManager<ThreadStaticSessionContext>.Current);
        protected override void RegisterDataAccessService() => RegisterSingleton<IDataAccessService, NHibernateDataAccess<ThreadStaticSessionContext>>();
    }

    // for more information see:
    // https://simpleinjector.readthedocs.io/en/latest/advanced.html#property-injection

    public class InjectPropertySelectionBehavior : IPropertySelectionBehavior
    {
        public bool SelectProperty(Type implementationType, PropertyInfo prop)
        {
            var result = prop.GetCustomAttributes(typeof(InjectAttribute)).Any();
            return result;
        }
    }
}
