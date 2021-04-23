namespace LNF.Billing.Reports.ServiceUnitBilling
{
    public abstract class JournalUnitReport : ReportBase
    {
        public JournalUnitReportItem[] Items { get; set; }
        public CreditEntry CreditEntry { get; set; }

        public override ReportTypes ReportType
        {
            get { return ReportTypes.JU; }
        }

        public JournalUnitTypes JournalUnitType { get; set; }

        public override string ReportName => $"{BillingCategory}-{ReportType}{JournalUnitType}";
    }
}
