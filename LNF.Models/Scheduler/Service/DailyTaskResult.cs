using LNF.Models.Billing.Process;

namespace LNF.Models.Scheduler.Service
{
    public class DailyTaskResult
    {
        public CheckExpiringClientsProcessResult CheckExpiringClientsProcessResult { get; set; }
        public CheckExpiredClientsProcessResult CheckExpiredClientsProcessResult { get; set; }
        public DataUpdateProcessResult DataUpdateProcessResult { get; set; }
        public BillingProcessStep1Result BillingProcessStep1Result { get; set; }
    }
}
