using System;

namespace LNF.Billing.Reports.ServiceUnitBilling
{
    public abstract class ReportBase
    {
        public int ClientID { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public abstract ReportTypes ReportType { get; }
        public abstract BillingCategory BillingCategory { get; }
    }
}
