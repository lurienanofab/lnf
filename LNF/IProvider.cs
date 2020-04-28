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

namespace LNF
{
    public interface IProvider
    {
        IAuthorizationService Authorization { get; }
        ILoggingService Log { get; }
        IControlService Control { get; }
        IDataService Data { get; }
        IBillingService Billing { get; }
        IInventoryService Inventory {get;}
        IOrderingService Ordering { get; }
        IStoreService Store { get; }
        IMailService Mail { get; }
        IPhysicalAccessService PhysicalAccess { get; }
        ISchedulerService Scheduler { get; }
        IFeedbackService Feedback { get; }
        IReportingService Reporting { get; }
        IWorkerService Worker { get; }
        IProviderUtility Utility { get; }
        IDataAccessService DataAccess { get; }
        string LoginUrl();
        bool IsProduction();
    }
}
