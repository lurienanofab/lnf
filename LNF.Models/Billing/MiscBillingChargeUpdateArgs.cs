using System;

namespace LNF.Models.Billing
{
    public class MiscBillingChargeUpdateArgs
    {
        public int ExpID { get; set; }
        public DateTime Period { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
