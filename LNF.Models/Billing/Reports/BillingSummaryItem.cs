using System;

namespace LNF.Models.Billing.Reports
{
    public class BillingSummaryItem
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public decimal TotalCharge { get; set; }
        public bool IncludeRemote { get; set; }
    }
}
