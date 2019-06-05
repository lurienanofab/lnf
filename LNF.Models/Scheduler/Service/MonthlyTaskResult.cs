using LNF.Models.Billing.Process;

namespace LNF.Models.Scheduler.Service
{
    public class MonthlyTaskResult
    {
        public int UserApportionmentReportCount { get; set; }
        public int FinancialManagerReportCount { get; set; }
        public int RoomAccessExpirationCheckCount { get; set; }
        public BillingProcessStep1Result BillingProcessStep1Result { get; set; }
        public PopulateSubsidyBillingProcessResult PopulateSubsidyBillingProcessResult { get; set; }
    }
}
