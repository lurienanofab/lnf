using System;

namespace LNF.Models.Data
{
    public class AccountItem
    {
        public int AccountID { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public string AccountName { get; set; }
        public string Number { get; set; }
        public string ShortCode { get; set; }
        public int FundingSourceID { get; set; }
        public string FundingSourceName { get; set; }
        public int TechnicalFieldID { get; set; }
        public string TechnicalFieldName { get; set; }
        public int SpecialTopicID { get; set; }
        public string SpecialTopicName { get; set; }
        public int BillAddressID { get; set; }
        public int ShipAddressID { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceLine1 { get; set; }
        public string InvoiceLine2 { get; set; }
        public DateTime? PoEndDate { get; set; }
        public decimal? PoInitialFunds { get; set; }
        public decimal? PoRemainingFunds { get; set; }
        public string Project { get; set; }
        public bool AccountActive { get; set; }
        public string FullAccountName { get; set; }
        public string NameWithShortCode { get; set; }
        public bool IsRegularAccountType { get; set; }

        public override string ToString()
        {
            return FullAccountName;
        }
    }
}
