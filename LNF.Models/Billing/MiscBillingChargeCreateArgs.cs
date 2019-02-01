using System;

namespace LNF.Models.Billing
{
    public class MiscBillingChargeCreateArgs
    {
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public DateTime Period { get; set; }
        public string SubType { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
