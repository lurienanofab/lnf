using System;

namespace LNF.Models.Billing
{
    public interface IAccountSubsidy
    {
        int AccountID { get; set; }
        int AccountSubsidyID { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? DisableDate { get; set; }
        DateTime EnableDate { get; set; }
        decimal UserPaymentPercentage { get; set; }
    }
}