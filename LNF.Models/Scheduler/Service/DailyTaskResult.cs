using LNF.Models.Billing.Process;

namespace LNF.Models.Scheduler.Service
{
    public class DailyTaskResult
    {
        public CheckExpiringClientsProcessResult CheckExpiringClientsProcessResult { get; set; }
        public CheckExpiredClientsProcessResult CheckExpiredClientsProcessResult { get; set; }
        public DataUpdateProcessResult DataUpdateProcessResult { get; set; }
        public PopulateToolBillingProcessResult PopulateToolBillingProcessResult { get; set; }
        public PopulateRoomBillingProcessResult PopulateRoomBillingProcessResult { get; set; }
        public PopulateStoreBillingProcessResult PopulateStoreBillingProcessResult { get; set; }
    }
}
