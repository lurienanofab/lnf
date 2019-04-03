using System;

namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents any object that includes account information
    /// </summary>
    public interface IAccountItem
    {
        /// <summary>
        /// The unique id of an Account
        /// </summary>
        int AccountID { get; set; }

        /// <summary>
        /// The name of an Account
        /// </summary>
        string AccountName { get; set; }

        /// <summary>
        /// The Account number
        /// </summary>
        string AccountNumber { get; set; }

        /// <summary>
        /// The Account ShortCode
        /// </summary>
        string ShortCode { get; set; }

        /// <summary>
        /// Id of the billing address
        /// </summary>
        int BillAddressID { get; set; }

        /// <summary>
        /// Id of the shipping address
        /// </summary>
        int ShipAddressID { get; set; }

        /// <summary>
        /// The Account invoice number
        /// </summary>
        string InvoiceNumber { get; set; }

        /// <summary>
        /// The Account invoice line 1
        /// </summary>
        string InvoiceLine1 { get; set; }

        /// <summary>
        /// The Account invoice line 2
        /// </summary>
        string InvoiceLine2 { get; set; }

        /// <summary>
        /// The end date of the Account PO
        /// </summary>
        DateTime? PoEndDate { get; set; }

        /// <summary>
        /// The Account initial funds
        /// </summary>
        decimal? PoInitialFunds { get; set; }

        /// <summary>
        /// The Account remaining funds
        /// </summary>
        decimal? PoRemainingFunds { get; set; }

        /// <summary>
        /// The Account project number - a predefined segment of Number
        /// </summary>
        string Project { get; }

        /// <summary>
        /// Indictes if a Account is currently active
        /// </summary>
        bool AccountActive { get; set; }

        /// <summary>
        /// Id used to indicate the funding source
        /// </summary>
        int FundingSourceID { get; set; }

        /// <summary>
        /// The assigned funding source value
        /// </summary>
        string FundingSourceName { get; set; }

        /// <summary>
        /// Id used to indicate the technical field
        /// </summary>
        int TechnicalFieldID { get; set; }

        /// <summary>
        /// The assigned technical field value
        /// </summary>
        string TechnicalFieldName { get; set; }

        /// <summary>
        /// Id used to indicate the special topic
        /// </summary>
        int SpecialTopicID { get; set; }

        /// <summary>
        /// The assigned special topic value
        /// </summary>
        string SpecialTopicName { get; set; }

        /// <summary>
        /// The unique id of an AccountType
        /// </summary>
        int AccountTypeID { get; set; }

        /// <summary>
        /// The name of a ChargeType
        /// </summary>
       string AccountTypeName { get; set; }
    }
}
