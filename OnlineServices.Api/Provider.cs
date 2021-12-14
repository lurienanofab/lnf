using LNF;
using RestSharp;
using System;
using System.Configuration;

namespace OnlineServices.Api
{
    public class Provider : IProvider
    {
        public LNF.Authorization.IAuthorizationService Authorization { get; }

        public LNF.Logging.ILoggingService Log { get; }

        public LNF.Control.IControlService Control { get; }

        public LNF.Data.IDataService Data { get; }

        public LNF.Billing.IBillingService Billing { get; }

        public LNF.Inventory.IInventoryService Inventory { get; }

        public LNF.Ordering.IOrderingService Ordering { get; }

        public LNF.Store.IStoreService Store { get; }

        public LNF.Mail.IMailService Mail { get; }

        public LNF.PhysicalAccess.IPhysicalAccessService PhysicalAccess { get; }

        public LNF.Scheduler.ISchedulerService Scheduler { get; }

        public LNF.Feedback.IFeedbackService Feedback { get; }

        public LNF.Reporting.IReportingService Reporting { get; }

        public LNF.Worker.IWorkerService Worker { get; }

        public LNF.Util.IProviderUtility Utility { get; }

        public LNF.DataAccess.IDataAccessService DataAccess => throw new Exception("This implementation does not use DataAccess.");

        public Provider(IRestClient rc)
        {
            Authorization = new Authorization.AuthorizationService(rc);
            Log = new Logging.LoggingService(rc);
            Control = new Control.ControlService(rc);
            Data = new Data.DataService(rc);
            Billing = new Billing.BillingService(rc);
            Inventory = new Inventory.InventoryService(rc);
            Ordering = new Ordering.OrderingService(rc);
            Store = new Store.StoreService(rc);
            Mail = new Mail.MailService(rc);
            PhysicalAccess = new PhysicalAccess.PhysicalAccessService(rc);
            Scheduler = new Scheduler.SchedulerService(rc);
            Feedback = new Feedback.FeedbackService(rc);
            Reporting = new Reporting.ReportingService(rc);
            Worker = new Worker.WorkerService(rc);
            Utility = new Utility.ProviderUtility(rc);
        }

        public bool IsProduction() => Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"]);

        public string LoginUrl() => ConfigurationManager.AppSettings["LoginUrl"];
    }
}