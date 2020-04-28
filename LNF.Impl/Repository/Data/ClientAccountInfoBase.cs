using LNF.Data;
using System;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// A base class for subclasses that share common Client and Account data
    /// </summary>
    public abstract class ClientAccountInfoBase : ClientOrgInfoBase, IClientAccount
    {
        /// <summary>
        /// The unique id of a ClientAccount
        /// </summary>
        public virtual int ClientAccountID { get; set; }

        /// <summary>
        /// Indicates if a ClientAccount is the default
        /// </summary>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Indicates if the user is a manager
        /// </summary>
        public virtual bool Manager { get; set; }

        /// <summary>
        /// Indicates if a ClientAccount is currently active
        /// </summary>
        public virtual bool ClientAccountActive { get; set; }

        /// <summary>
        /// The unique id of an Account
        /// </summary>
        public virtual int AccountID { get; set; }

        /// <summary>
        /// The name of an Account
        /// </summary>
        public virtual string AccountName { get; set; }

        /// <summary>
        /// The Account number
        /// </summary>
        public virtual string AccountNumber { get; set; }

        /// <summary>
        /// The Account ShortCode
        /// </summary>
        public virtual string ShortCode { get; set; }

        /// <summary>
        /// Id of the billing address
        /// </summary>
        public virtual int BillAddressID { get; set; }

        /// <summary>
        /// Id of the shipping address
        /// </summary>
        public virtual int ShipAddressID { get; set; }

        /// <summary>
        /// The Account invoice number
        /// </summary>
        public virtual string InvoiceNumber { get; set; }

        /// <summary>
        /// The Account invoice line 1
        /// </summary>
        public virtual string InvoiceLine1 { get; set; }

        /// <summary>
        /// The Account invoice line 2
        /// </summary>
        public virtual string InvoiceLine2 { get; set; }

        /// <summary>
        /// The end date of the Account PO
        /// </summary>
        public virtual DateTime? PoEndDate { get; set; }

        /// <summary>
        /// The Account initial funds
        /// </summary>
        public virtual decimal? PoInitialFunds { get; set; }

        /// <summary>
        /// The Account remaining funds
        /// </summary>
        public virtual decimal? PoRemainingFunds { get; set; }

        /// <summary>
        /// The Account project number - a predefined segment of Number
        /// </summary>
        public virtual string Project => Accounts.GetProject(AccountNumber);

        /// <summary>
        /// Indictes if a Account is currently active
        /// </summary>
        public virtual bool AccountActive { get; set; }

        /// <summary>
        /// Id used to indicate the funding source
        /// </summary>
        public virtual int FundingSourceID { get; set; }

        /// <summary>
        /// The assigned funding source value
        /// </summary>
        public virtual string FundingSourceName { get; set; }

        public virtual int TechnicalFieldID { get; set; }

        public virtual string TechnicalFieldName { get; set; }

        /// <summary>
        /// Id used to indicate the special topic
        /// </summary>
        public virtual int SpecialTopicID { get; set; }

        /// <summary>
        /// The assigned special topic value
        /// </summary>
        public virtual string SpecialTopicName { get; set; }

        /// <summary>
        /// The unique id of an AccountType
        /// </summary>
        public virtual int AccountTypeID { get; set; }

        /// <summary>
        /// The name of a ChargeType
        /// </summary>
        public virtual string AccountTypeName { get; set; }

        public virtual string NameWithShortCode => Accounts.GetNameWithShortCode(AccountName, ShortCode);

        public virtual string FullAccountName => Accounts.GetFullAccountName(AccountName, ShortCode, OrgName);

        public virtual bool IsRegularAccountType => Accounts.GetIsRegularAccountType(AccountTypeID);
    }
}
