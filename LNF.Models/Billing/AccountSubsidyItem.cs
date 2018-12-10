using System;

namespace LNF.Models.Billing
{
    public class AccountSubsidyItem
    {
        public int AccountSubsidyID { get; set; }
        public int AccountID { get; set; }
        public decimal UserPaymentPercentage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EnableDate { get; set; }
        public DateTime? DisableDate { get; set; }
    }
}
