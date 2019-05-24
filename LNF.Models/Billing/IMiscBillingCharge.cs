using System;

namespace LNF.Models.Billing
{
    public interface IMiscBillingCharge
    {
        int AccountID { get; set; }
        string AccountName { get; set; }
        DateTime ActDate { get; set; }
        bool Active { get; set; }
        int ClientID { get; set; }
        string Description { get; set; }
        string DisplayName { get; }
        int ExpID { get; set; }
        string FName { get; set; }
        string LName { get; set; }
        DateTime Period { get; set; }
        double Quantity { get; set; }
        string ShortCode { get; set; }
        decimal SubsidyDiscount { get; set; }
        string SUBType { get; set; }
        decimal TotalCost { get; }
        decimal UnitCost { get; set; }
        decimal UserPayment { get; }
    }
}