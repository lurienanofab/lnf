using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Models.Mail;
using LNF.Models.PhysicalAccess;
using LNF.Models.Scheduler;
using LNF.Models.Worker;

namespace LNF.Models
{
    public interface IProvider
    {
        IDataService Data { get; }
        IBillingServices Billing { get; }
        IMailService Mail { get; }
        IPhysicalAccessService PhysicalAccess { get; }
        ISchedulerService Scheduler { get; }
        IWorkerService Worker { get; }
        bool IsProduction();
    }
}
