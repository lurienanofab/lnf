using System;

namespace LNF.Models.Billing
{
    public class StoreBillingItem
    {
        public int StoreBillingID { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public int ChargeTypeID { get; set; }
        public int ItemID { get; set; }
        public int CategoryID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal CostMultiplier { get; set; }
        public decimal LineCost { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public bool IsTemp { get; set; }
    }
}
