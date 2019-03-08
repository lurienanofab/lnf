using System;

namespace LNF.Models.Data
{
    public class AccountItem : IAccount
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
        public bool AccountActive { get; set; }
        public string Project => GetProject(Number);
        public string NameWithShortCode => GetNameWithShortCode(AccountName, ShortCode);
        public string FullAccountName => GetFullAccountName(AccountName, ShortCode, OrgName);
        public bool IsRegularAccountType => GetIsRegularAccountType(AccountTypeID);

        public override string ToString()
        {
            return FullAccountName;
        }

        public static string GetProject(string number)
        {
            if (string.IsNullOrEmpty(number))
                return string.Empty;

            return number.Substring(number.Length - 7);
        }

        public static string GetNameWithShortCode(string accountName, string shortCode)
        {
            string result = accountName;

            if (!string.IsNullOrEmpty(shortCode.Trim()))
                result = "[" + shortCode.Trim() + "] " + result;

            return result.Trim();
        }

        public static string GetFullAccountName(string accountName, string shortCode, string orgName)
        {
            string result = GetNameWithShortCode(accountName, shortCode);

            if (!string.IsNullOrEmpty(orgName))
                result += " (" + orgName + ")";

            return result.Trim();
        }

        public static bool GetIsRegularAccountType(int accountTypeId) => accountTypeId == 1;
    }
}
