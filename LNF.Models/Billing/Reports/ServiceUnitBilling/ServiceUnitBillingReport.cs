namespace LNF.Models.Billing.Reports.ServiceUnitBilling
{
    public abstract class ServiceUnitBillingReport : ReportBase
    {
        public ServiceUnitBillingReportItem[][] Items { get; set; }
        public ServiceUnitBillingReportItem[] CombinedItems { get; set; }
        public BillingUnit[] Summaries { get; set; }

        public override ReportTypes ReportType
        {
            get { return ReportTypes.SUB; }
        }
    }
}
