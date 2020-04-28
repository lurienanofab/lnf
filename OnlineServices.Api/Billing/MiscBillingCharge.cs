using LNF.Billing;
using System;

namespace OnlineServices.Api.Billing
{
    public class MiscBillingCharge : IMiscBillingCharge
    {
        public int ExpID { get; set; }
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public string SUBType { get; set; }
        public DateTime Period { get; set; }
        public DateTime ActDate { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal SubsidyDiscount { get; set; }
        public bool Active { get; set; }
        public virtual decimal TotalCost => Convert.ToDecimal(Quantity) * UnitCost;
        public virtual decimal UserPayment => TotalCost - SubsidyDiscount;
    }
}
