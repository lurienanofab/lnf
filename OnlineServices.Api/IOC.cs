﻿using LNF.Models;
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
using OnlineServices.Api.Authorization;
using OnlineServices.Api.Billing;
using OnlineServices.Api.Data;
using OnlineServices.Api.Mail;
using OnlineServices.Api.PhysicalAccess;
using OnlineServices.Api.Scheduler;
using OnlineServices.Api.Worker;
using SimpleInjector;

namespace OnlineServices.Api
{
    public class IOC
    {
        public static void Configure(Container container)
        {
            // ***** Data *****************************************************
            container.Register<IDataService, DataService>(Lifestyle.Scoped);
            container.Register<IClientManager, ClientManager>(Lifestyle.Scoped);
            container.Register<IOrgManager, OrgManager>(Lifestyle.Scoped);
            container.Register<IActiveLogManager, ActiveLogManager>(Lifestyle.Scoped);
            container.Register<ICostManager, CostManager>(Lifestyle.Scoped);
            container.Register<IDryBoxManager, DryBoxManager>(Lifestyle.Scoped);
            container.Register<IAccountManager, AccountManager>(Lifestyle.Scoped);
            container.Register<IRoomManager, RoomManager>(Lifestyle.Scoped);
            container.Register<IServiceLogManager, ServiceLogManager>(Lifestyle.Scoped);
            container.Register<IUtilityManager, UtilityManager>(Lifestyle.Scoped);
            container.Register<IFeedManager, FeedManager>(Lifestyle.Scoped);

            // ***** Billing **************************************************
            container.Register<IBillingServices, BillingService>(Lifestyle.Scoped);
            container.Register<IAccountSubsidyManager, AccountSubsidyManager>(Lifestyle.Scoped);
            container.Register<IProcessManager, ProcessManager>(Lifestyle.Scoped);
            container.Register<IReportManager, ReportManager>(Lifestyle.Scoped);
            container.Register<IToolBillingManager, ToolBillingManager>(Lifestyle.Scoped);
            container.Register<IRoomBillingManager, RoomBillingManager>(Lifestyle.Scoped);
            container.Register<IStoreBillingManager, StoreBillingManager>(Lifestyle.Scoped);
            container.Register<IMiscBillingManager, MiscBillingManager>(Lifestyle.Scoped);
            container.Register<IApportionmentManager, ApportionmentManager>(Lifestyle.Scoped);
            container.Register<IBillingTypeManager, BillingTypeManager>(Lifestyle.Scoped);

            // ***** Scheduler ************************************************
            container.Register<ISchedulerService, SchedulerService>(Lifestyle.Scoped);
            container.Register<IResourceManager, ResourceManager>(Lifestyle.Scoped);
            container.Register<IReservationManager, ReservationManager>(Lifestyle.Scoped);

            container.Register<IMailService, MailService>(Lifestyle.Scoped);

            container.Register<IPhysicalAccessService, PhysicalAccessService>(Lifestyle.Scoped);

            container.Register<IWorkerService, WorkerService>(Lifestyle.Scoped);

            container.Register<IAuthorizationService, AuthorizationService>(Lifestyle.Scoped);

            container.Register<IProvider, ServiceProvider>(Lifestyle.Scoped);
        }
    }
}