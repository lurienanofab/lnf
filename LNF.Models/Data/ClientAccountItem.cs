using System;

namespace LNF.Models.Data
{
    public class ClientAccountItem : ClientItem, IClientAccount
    {
        public int ClientAccountID { get; set; }
        public bool IsDefault { get; set; }
        public bool Manager { get; set; }
        public bool ClientAccountActive { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string ShortCode { get; set; }
        public int BillAddressID { get; set; }
        public int ShipAddressID { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceLine1 { get; set; }
        public string InvoiceLine2 { get; set; }
        public DateTime? PoEndDate { get; set; }
        public decimal? PoInitialFunds { get; set; }
        public decimal? PoRemainingFunds { get; set; }
        public bool AccountActive { get; set; }
        public int FundingSourceID { get; set; }
        public string FundingSourceName { get; set; }
        public int TechnicalFieldID { get; set; }
        public string TechnicalFieldName { get; set; }
        public int SpecialTopicID { get; set; }
        public string SpecialTopicName { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public string Project => AccountItem.GetProject(AccountNumber);
        public string NameWithShortCode => AccountItem.GetNameWithShortCode(AccountName, ShortCode);
        public string FullAccountName => AccountItem.GetFullAccountName(AccountName, ShortCode, OrgName);
        public bool IsRegularAccountType => AccountItem.GetIsRegularAccountType(AccountTypeID);
    }
}
