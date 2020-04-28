using LNF.Authorization;
using LNF.Billing;
using LNF.Control;
using LNF.Data;
using LNF.DataAccess;
using LNF.Feedback;
using LNF.Inventory;
using LNF.Logging;
using LNF.Mail;
using LNF.Ordering;
using LNF.PhysicalAccess;
using LNF.Reporting;
using LNF.Scheduler;
using LNF.Store;
using LNF.Util;
using LNF.Worker;

namespace LNF.Impl
{
    public class Provider : IProvider
    {
        public IAuthorizationService Authorization { get; }
        public ILoggingService Log { get; }
        public IControlService Control { get; }
        public IDataService Data { get; }
        public IBillingService Billing { get; }
        public IInventoryService Inventory { get; }
        public IOrderingService Ordering { get; }
        public IStoreService Store { get; }
        public IMailService Mail { get; }
        public IPhysicalAccessService PhysicalAccess { get; }
        public ISchedulerService Scheduler { get; }
        public IFeedbackService Feedback { get; }
        public IReportingService Reporting { get; }
        public IWorkerService Worker { get; }
        public IProviderUtility Utility { get; }
        public IDataAccessService DataAccess { get; } // I give up.

        public Provider(
            IAuthorizationService authorization,
            ILoggingService log,
            IControlService control,
            IDataService data,
            IBillingService billing,
            IInventoryService inventory,
            IOrderingService ordering,
            IStoreService store,
            IMailService mail,
            IPhysicalAccessService physicalAccess,
            ISchedulerService scheduler,
            IFeedbackService feedback,
            IReportingService reporting,
            IWorkerService worker,
            IProviderUtility utility,
            IDataAccessService dataAccess)
        {
            Authorization = authorization;
            Log = log;
            Control = control;
            Data = data;
            Billing = billing;
            Inventory = inventory;
            Ordering = ordering;
            Store = store;
            Mail = mail;
            PhysicalAccess = physicalAccess;
            Scheduler = scheduler;
            Feedback = feedback;
            Reporting = reporting;
            Worker = worker;
            Utility = utility;
            DataAccess = dataAccess;
        }

        public string LoginUrl() => Configuration.Current.Context.LoginUrl;
        public bool IsProduction() => Configuration.Current.Production;
    }
}
