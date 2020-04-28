using System;

namespace LNF.Billing
{
    public interface IAccountSubsidy
    {
        int AccountSubsidyID { get; set; }
        int AccountID { get; set; }
        decimal UserPaymentPercentage { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime EnableDate { get; set; }
        DateTime? DisableDate { get; set; }
    }
}