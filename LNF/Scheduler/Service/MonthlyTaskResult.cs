using LNF.Billing.Process;

namespace LNF.Scheduler.Service
{
    public class MonthlyTaskResult
    {
        public int UserApportionmentReportCount { get; set; }
        public int FinancialManagerReportCount { get; set; }
        public int RoomAccessExpirationCheckCount { get; set; }
        public Step1Result BillingProcessStep1Result { get; set; }
        public PopulateSubsidyBillingResult PopulateSubsidyBillingProcessResult { get; set; }
    }
}
