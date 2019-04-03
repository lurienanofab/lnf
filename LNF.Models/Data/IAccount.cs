using System;

namespace LNF.Models.Data
{
    public interface IAccount : IOrg
    {
        int AccountID { get; set; }
        string AccountName { get; set; }
        string AccountNumber { get; set; }
        string ShortCode { get; set; }
        int BillAddressID { get; set; }
        int ShipAddressID { get; set; }
        string InvoiceNumber { get; set; }
        string InvoiceLine1 { get; set; }
        string InvoiceLine2 { get; set; }
        DateTime? PoEndDate { get; set; }
        decimal? PoInitialFunds { get; set; }
        decimal? PoRemainingFunds { get; set; }
        bool AccountActive { get; set; }
        int FundingSourceID { get; set; }
        string FundingSourceName { get; set; }
        int TechnicalFieldID { get; set; }
        string TechnicalFieldName { get; set; }
        int SpecialTopicID { get; set; }
        string SpecialTopicName { get; set; }
        int AccountTypeID { get; set; }
        string AccountTypeName { get; set; }
        string Project { get; }
        string NameWithShortCode { get; }
        string FullAccountName { get; }
        bool IsRegularAccountType { get; }
    }
}
