using LNF.Data;
using System;

namespace OnlineServices.Api.Data
{
    public class Account : IAccount
    {
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
        public string Project => Accounts.GetProject(AccountNumber);
        public bool AccountActive { get; set; }
        public int FundingSourceID { get; set; }
        public string FundingSourceName { get; set; }
        public int TechnicalFieldID { get; set; }
        public string TechnicalFieldName { get; set; }
        public int SpecialTopicID { get; set; }
        public string SpecialTopicName { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public string NameWithShortCode => Accounts.GetNameWithShortCode(AccountName, ShortCode);
        public string FullAccountName => Accounts.GetFullAccountName(AccountName, ShortCode, OrgName);
        public bool IsRegularAccountType => Accounts.GetIsRegularAccountType(AccountTypeID);
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public int DefClientAddressID { get; set; }
        public int DefBillAddressID { get; set; }
        public int DefShipAddressID { get; set; }
        public bool NNINOrg { get; set; }
        public bool PrimaryOrg { get; set; }
        public bool OrgActive { get; set; }
        public int OrgTypeID { get; set; }
        public string OrgTypeName { get; set; }
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public int ChargeTypeAccountID { get; set; }
    }
}
