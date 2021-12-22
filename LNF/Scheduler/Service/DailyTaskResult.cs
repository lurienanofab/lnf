using LNF.Billing.Process;

namespace LNF.Scheduler.Service
{
    public class DailyTaskResult
    {
        public CheckExpiringClientsProcessResult CheckExpiringClientsProcessResult { get; set; }
        public CheckExpiredClientsProcessResult CheckExpiredClientsProcessResult { get; set; }
        public UpdateResult DataUpdateProcessResult { get; set; }
        public Step1Result BillingProcessStep1Result { get; set; }
    }
}
