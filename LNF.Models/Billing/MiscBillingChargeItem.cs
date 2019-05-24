using LNF.Models.Data;
using System;

namespace LNF.Models.Billing
{
    public class MiscBillingChargeItem : IMiscBillingCharge
    {
        public int ExpID { get; set; }
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public string SUBType { get; set; }
        public DateTime Period { get; set; }
        public DateTime ActDate { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal SubsidyDiscount { get; set; }
        public bool Active { get; set; }
        public string DisplayName => ClientItem.GetDisplayName(LName, FName);
        public decimal TotalCost => Convert.ToDecimal(Quantity) * UnitCost;
        public decimal UserPayment => TotalCost - SubsidyDiscount;
    }
}
