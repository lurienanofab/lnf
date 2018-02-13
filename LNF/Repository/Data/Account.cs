using LNF.Data;
using LNF.Models.Data;
using System;

namespace LNF.Repository.Data
{
    /// <summary>
    /// A billing account
    /// </summary>
    public class Account : ActiveDataItem
    {
        /// <summary>
        /// The unique id of an Account
        /// </summary>
        public virtual int AccountID { get; set; }

        /// <summary></summary>
        public virtual Org Org { get; set; }

        /// <summary></summary>
        public virtual AccountType AccountType { get; set; }

        /// <summary>
        /// The name of an Account
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The Account number
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// The Account ShortCode
        /// </summary>
        public virtual string ShortCode { get; set; }

        /// <summary>
        /// Id used to indicate the funding source
        /// </summary>
        public virtual int FundingSourceID { get; set; }

        /// <summary>
        /// Id used to indicate the technical field
        /// </summary>
        public virtual int TechnicalFieldID { get; set; }

        /// <summary>
        /// Id used to indicate the special topic
        /// </summary>
        public virtual int SpecialTopicID { get; set; }

        /// <summary>
        /// Id used to indicate the billing address
        /// </summary>
        public virtual int BillAddressID { get; set; }

        /// <summary>
        /// Id used to indicate the shipping address
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
        /// The Account PO initial funds
        /// </summary>
        public virtual decimal? PoInitialFunds { get; set; }

        /// <summary>
        /// The Account PO remaining funds
        /// </summary>
        public virtual decimal? PoRemainingFunds { get; set; }

        /// <summary>
        /// The Account project number - a predefined segment of Number
        /// </summary>
        public virtual string Project { get { return Account.GetProject(Number); } protected set { } }

        /// <summary>
        /// Indictes if a Account is currently active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// The table name used in the ActiveLog
        /// </summary>
        public override string TableName() { return "Account"; }

        /// <summary>
        /// Gets the record used in the ActiveLog
        /// </summary>
        /// <returns>The AccountID</returns>
        public override int Record() { return AccountID; }

        public virtual AccountChartFields GetChartFields()
        {
            return new AccountChartFields(this);
        }

        /// <summary>
        /// Gets the full name of the account - including ShortCode, Name, and OrgName
        /// </summary>
        /// <returns>The string value of the full account name</returns>
        public virtual string GetFullAccountName()
        {
            return GetFullAccountName(ShortCode, Name, Org.OrgName);
        }

        /// <summary>
        /// Gets the full name of the account - including ShortCode, Name, and OrgName
        /// </summary>
        /// <returns>The string value of the full account name</returns>
        public override string ToString()
        {
            return GetFullAccountName();
        }

        /// <summary>
        /// Gets the account name with the ShortCode
        /// </summary>
        /// <returns>The string value of the account name with the ShortCode</returns>
        public virtual string GetNameWithShortCode()
        {
            return Account.GetNameWithShortCode(ShortCode, Name);
        }

        /// <summary>
        /// Get the project component of an account Number
        /// </summary>
        /// <returns></returns>
        public virtual string GetProject()
        {
            return Account.GetProject(Number);
        }


        /// <summary>
        /// Gets a value indicating if the associated AccountType is the regular type
        /// </summary>
        /// <returns>True if the AccountType is regular, otherwise false</returns>
        public virtual bool IsRegularAccountType()
        {
            return AccountType.AccountTypeID == 1;
        }

        /// <summary>
        /// Gets the name of the account. The name includes the account ShortCode (if applicable), account Name and OrgName.
        /// </summary>
        public static string GetFullAccountName(string shortcode, string accountName, string orgName)
        {
            string result = GetNameWithShortCode(shortcode, accountName);

            if (!string.IsNullOrEmpty(orgName))
                result += " (" + orgName + ")";

            return result.Trim();
        }

        /// <summary>
        /// Concatenates an account name and a ShortCode
        /// </summary>
        /// <param name="shortcode">The account ShortCode</param>
        /// <param name="accountName">The account name</param>
        /// <returns>A formatted string using the given ShortCode and account name values</returns>
        public static string GetNameWithShortCode(string shortcode, string accountName)
        {
            string result = accountName;

            if (!string.IsNullOrEmpty(shortcode.Trim()))
                result = "[" + shortcode.Trim() + "] " + result;

            return result.Trim();
        }

        /// <summary>
        /// Gets the Project compoenent from an account number
        /// </summary>
        /// <param name="number">The account number</param>
        /// <returns>A string value for the account Project</returns>
        public static string GetProject(string number)
        {
            if (string.IsNullOrEmpty(number))
                return string.Empty;

            return number.Substring(number.Length - 7);
        }
    }
}
