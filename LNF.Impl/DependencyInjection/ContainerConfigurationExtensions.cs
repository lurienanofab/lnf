using LNF.Authorization;
using LNF.Billing;
using LNF.Billing.Process;
using LNF.Billing.Reports;
using LNF.Cache;
using LNF.Control;
using LNF.Data;
using LNF.Feedback;
using LNF.Impl.Authorization;
using LNF.Impl.Billing;
using LNF.Impl.Billing.Report;
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

namespace LNF.Impl.DependencyInjection
{
    public static class ContainerConfigurationExtensions
    {
        public static void RegisterAllTypes(this ContainerConfiguration config)
        {
            // Context API
            config.RegisterContext();

            // DataAccess API
            config.RegisterSessionManager();

            if (!config.SkipDataAccessRegistration)
                config.RegisterDataAccessService();

            config.Context.RegisterDisposableTransient(typeof(NHibernateUnitOfWork), "NHibernateMiddleware creates and disposes IUnitOfWork classes for us.");

            //Register<IUnitOfWork, NHibernateUnitOfWork>();  // not a singleton

            // Authorization API
            config.Context.RegisterSingleton<IAuthorizationService, AuthorizationService>();

            // Logging API
            config.Context.RegisterSingleton<ILoggingService, LoggingService>();

            // Control API
            config.Context.RegisterSingleton<IControlService, WagoControlService>();

            // Scripting API
            config.Context.RegisterSingleton<IScriptEngine, DefaultScriptEngine>();

            // Data API
            config.Context.RegisterSingleton<IDataService, DataService>();
            config.Context.RegisterSingleton<IAccountRepository, AccountRepository>();
            config.Context.RegisterSingleton<IClientRepository, ClientRepository>();
            config.Context.RegisterSingleton<IOrgRepository, OrgRepository>();
            config.Context.RegisterSingleton<IActiveLogRepository, ActiveLogRepository>();
            config.Context.RegisterSingleton<ICostRepository, CostRepository>();
            config.Context.RegisterSingleton<IDryBoxRepository, DryBoxRepository>();
            config.Context.RegisterSingleton<IRoomRepository, RoomRepository>();
            config.Context.RegisterSingleton<IServiceLogRepository, ServiceLogRepository>();
            config.Context.RegisterSingleton<IFeedRepository, FeedRepository>();
            config.Context.RegisterSingleton<IHolidayRepository, HolidayRepository>();
            config.Context.RegisterSingleton<IHelpRepository, HelpRepository>();
            config.Context.RegisterSingleton<IGlobalSettingRepository, GlobalSettingRepository>();
            config.Context.RegisterSingleton<IMenuRepository, MenuRepository>();
            config.Context.RegisterSingleton<INewsRepository, NewsRepository>();

            // Billing API
            config.Context.RegisterSingleton<IBillingService, BillingService>();
            config.Context.RegisterSingleton<ISubsidyRepository, SubsidyRepository>();
            config.Context.RegisterSingleton<IProcessRepository, ProcessRepository>();
            config.Context.RegisterSingleton<IReportRepository, ReportRepository>();
            config.Context.RegisterSingleton<IToolBillingRepository, ToolBillingRepository>();
            config.Context.RegisterSingleton<IRoomBillingRepository, RoomBillingRepository>();
            config.Context.RegisterSingleton<IStoreBillingRepository, StoreBillingRepository>();
            config.Context.RegisterSingleton<IMiscBillingRepository, MiscBillingRepository>();
            config.Context.RegisterSingleton<IBillingTypeRepository, BillingTypeRepository>();
            config.Context.RegisterSingleton<IApportionmentRepository, ApportionmentRepository>();
            config.Context.RegisterSingleton<IToolDataRepository, ToolDataRepository>();
            config.Context.RegisterSingleton<IRoomDataRepository, RoomDataRepository>();
            config.Context.RegisterSingleton<IStoreDataRepository, StoreDataRepository>();
            config.Context.RegisterSingleton<IMiscDataRepository, MiscDataRepository>();
            config.Context.RegisterSingleton<IOrgRechargeRepository, OrgRechargeRepository>();
            config.Context.RegisterSingleton<IExternalInvoiceRepository, ExternalInvoiceRepository>();

            // Inventory API
            config.Context.RegisterSingleton<IInventoryService, InventoryService>();
            config.Context.RegisterSingleton<IInventoryItemRepository, InventoryItemRepository>();
            config.Context.RegisterSingleton<ICategoryRepository, CategoryRepository>();

            // Ordering API
            config.Context.RegisterSingleton<IOrderingService, OrderingService>();
            config.Context.RegisterSingleton<IPurchaseOrderRepository, PurchaseOrderRepository>();
            config.Context.RegisterSingleton<IPurchaseOrderItemRepository, PurchaseOrderItemRepository>();
            config.Context.RegisterSingleton<IPurchaseOrderCategoryRepository, PurchaseOrderCategoryRepository>();
            config.Context.RegisterSingleton<IVendorRepository, VendorRepository>();
            config.Context.RegisterSingleton<IApproverRepository, ApproverRepository>();
            config.Context.RegisterSingleton<IPurchaserRepository, PurchaserRepository>();
            config.Context.RegisterSingleton<ITrackingRepository, TrackingRepository>();

            // Store API
            config.Context.RegisterSingleton<IStoreService, StoreService>();

            // Mail API
            config.Context.RegisterSingleton<IMailService, MailService>();
            config.Context.RegisterSingleton<IMassEmailRepository, MassEmailRepository>();
            config.Context.RegisterSingleton<IAttachmentUtility, AttachmentUtility>();

            // PhysicalAccess API
            config.Context.RegisterSingleton<IPhysicalAccessService, PhysicalAccessService>();

            // Scheduler API
            config.Context.RegisterSingleton<ISchedulerService, SchedulerService>();
            config.Context.RegisterSingleton<IProcessInfoRepository, ProcessInfoRepository>();
            config.Context.RegisterSingleton<IReservationRepository, ReservationRepository>();
            config.Context.RegisterSingleton<IResourceRepository, ResourceRepository>();
            config.Context.RegisterSingleton<IEmailRepository, EmailRepository>();
            config.Context.RegisterSingleton<IClientSettingRepository, ClientSettingRepository>();
            config.Context.RegisterSingleton<IActivityRepository, ActivityRepository>();
            config.Context.RegisterSingleton<IKioskRepository, KioskRepository>();
            config.Context.RegisterSingleton<ISchedulerPropertyRepository, SchedulerPropertyRepository>();
            config.Context.RegisterSingleton<ILabLocationRepository, LabLocationRepository>();

            // Feedback API
            config.Context.RegisterSingleton<IFeedbackService, FeedbackService>();

            // Worker API
            config.Context.RegisterSingleton<IWorkerService, WorkerService>();

            // Reporting API
            config.Context.RegisterSingleton<IReportingService, ReportingService>();
            config.Context.RegisterSingleton<IClientItemRepository, ClientItemRepository>();
            config.Context.RegisterSingleton<IClientManagerLogRepository, ClientManagerLogRepository>();
            config.Context.RegisterSingleton<IClientEmailPreferenceRepository, ClientEmailPreferenceRepository>();
            config.Context.RegisterSingleton<IManagerUsageChargeRepository, ManagerUsageChargeRepository>();
            config.Context.RegisterSingleton<IAfterHoursRepository, AfterHoursRepository>();

            // Provider Utility
            config.Context.RegisterSingleton<IProviderUtility, ProviderUtility>();
            config.Context.RegisterSingleton<IAutoEndUtility, AutoEndUtility>();
            config.Context.RegisterSingleton<IEncryptionUtility, EncryptionUtility>();
            config.Context.RegisterSingleton<ISerializationUtility, SerializationUtility>();
            config.Context.RegisterSingleton<ISiteMenuUtility, SiteMenuUtility>();

            // Tie it all together
            config.Context.RegisterSingleton<IProvider, Provider>();
            config.Context.RegisterSingleton<ICache, DefaultCache>();
        }
    }

}
