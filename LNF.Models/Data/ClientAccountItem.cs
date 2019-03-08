using System;

namespace LNF.Models.Data
{
    public class ClientAccountItem : IPrivileged, IAccount
    {
        public int ClientAccountID { get; set; }
        public bool IsDefault { get; set; }
        public bool Manager { get; set; }
        public bool ClientAccountActive { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string Number { get; set; }
        public string ShortCode { get; set; }
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
        public int FundingSourceID { get; set; }
        public string FundingSourceName { get; set; }
        public int TechnicalFieldID { get; set; }
        public string TechnicalFieldName { get; set; }
        public int SpecialTopicID { get; set; }
        public string SpecialTopicName { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public int ClientOrgID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
        public bool ClientOrgActive { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public bool OrgActive { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string MName { get; set; }
        public string FName { get; set; }
        public string DisplayName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public bool ClientActive { get; set; }
        public string FullAccountName => AccountItem.GetFullAccountName(AccountName, ShortCode, OrgName);
        public string NameWithShortCode => AccountItem.GetNameWithShortCode(AccountName, ShortCode);
        public bool IsRegularAccountType => AccountItem.GetIsRegularAccountType(AccountTypeID);
    }
}
