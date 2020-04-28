using System;

namespace LNF.Billing
{
    public interface IMiscBillingCharge
    {
        int ExpID { get; set; }
        int ClientID { get; set; }
        int AccountID { get; set; }
        string SUBType { get; set; }
        DateTime Period { get; set; }
        DateTime ActDate { get; set; }
        string Description { get; set; }
        double Quantity { get; set; }
        decimal UnitCost { get; set; }
        decimal SubsidyDiscount { get; set; }
        bool Active { get; set; }
        
        /// <summary>
        /// The total charge used to calculate subsidy.
        /// </summary>
        decimal TotalCost { get; }

        decimal UserPayment { get; }
    }
}