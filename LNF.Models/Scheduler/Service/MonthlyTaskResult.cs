using LNF.Models.Billing.Process;

namespace LNF.Models.Scheduler.Service
{
    public class MonthlyTaskResult
    {
        public int UserApportionmentReportCount { get; set; }
        public int FinancialManagerReportCount { get; set; }
        public int RoomAccessExpirationCheckCount { get; set; }
        public PopulateToolBillingProcessResult PopulateToolBillingProcessResult { get; set; }
        public PopulateRoomBillingProcessResult PopulateRoomBillingProcessResult { get; set; }
        public PopulateStoreBillingProcessResult PopulateStoreBillingProcessResult { get; set; }
        public PopulateSubsidyBillingProcessResult PopulateSubsidyBillingProcessResult { get; set; }
    }
}
