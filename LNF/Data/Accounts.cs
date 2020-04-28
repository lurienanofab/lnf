using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public static class Accounts
    {
        public static DataTable ConvertToAccountTable(IEnumerable<IAccount> accounts)
        {
            var dt = new DataTable();
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Name", typeof(string));

            foreach (var a in accounts.OrderBy(x => x.AccountName))
                dt.Rows.Add(a.AccountID, a.AccountName);

            return dt;
        }

        public static IList<IAccount> ConvertToAccountList(DataTable dt, Func<IAccount> newAcct)
        {
            List<IAccount> result = new List<IAccount>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var acct = newAcct();

                    if (dr.Table.Columns.Contains("AccountID"))
                        acct.AccountID = Convert.ToInt32(dr["AccountID"]);

                    if (dr.Table.Columns.Contains("OrgID"))
                    {
                        var org = Orgs.All().First(x => x.OrgID == Convert.ToInt32(dr["OrgID"]));
                        acct.OrgID = org.OrgID;
                        acct.OrgName = org.OrgName;
                    }

                    if (dr.Table.Columns.Contains("Name"))
                        acct.AccountName = (dr["Name"] == DBNull.Value) ? null : dr["Name"].ToString();

                    if (dr.Table.Columns.Contains("AccountTypeID"))
                    {
                        var acctType = AccountTypes.All().First(x => x.AccountTypeID == Convert.ToInt32(dr["AccountTypeID"]));
                        acct.AccountTypeID = acctType.AccountTypeID;
                        acct.AccountTypeName = acctType.AccountTypeName;
                    }

                    if (dr.Table.Columns.Contains("Number"))
                        acct.AccountNumber = (dr["Number"] == DBNull.Value) ? null : dr["Number"].ToString();

                    if (dr.Table.Columns.Contains("ShortCode"))
                        acct.ShortCode = (dr["ShortCode"] == DBNull.Value) ? null : dr["ShortCode"].ToString();

                    if (dr.Table.Columns.Contains("FundingSourceID"))
                        acct.FundingSourceID = (dr["FundingSourceID"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["FundingSourceID"]);

                    if (dr.Table.Columns.Contains("TechnicalFieldID"))
                        acct.TechnicalFieldID = (dr["TechnicalFieldID"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["TechnicalFieldID"]);

                    if (dr.Table.Columns.Contains("SpecialTopicID"))
                        acct.SpecialTopicID = (dr["SpecialTopicID"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["SpecialTopicID"]);

                    if (dr.Table.Columns.Contains("BillAddressID"))
                        acct.BillAddressID = (dr["BillAddressID"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["BillAddressID"]);

                    if (dr.Table.Columns.Contains("ShipAddressID"))
                        acct.ShipAddressID = (dr["ShipAddressID"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["ShipAddressID"]);

                    if (dr.Table.Columns.Contains("InvoiceNumber"))
                        acct.InvoiceNumber = (dr["InvoiceNumber"] == DBNull.Value) ? null : dr["InvoiceNumber"].ToString();

                    if (dr.Table.Columns.Contains("InvoiceLine1"))
                        acct.InvoiceLine1 = (dr["InvoiceLine1"] == DBNull.Value) ? null : dr["InvoiceLine1"].ToString();

                    if (dr.Table.Columns.Contains("InvoiceLine2"))
                        acct.InvoiceLine2 = (dr["InvoiceLine2"] == DBNull.Value) ? null : dr["InvoiceLine2"].ToString();

                    if (dr.Table.Columns.Contains("PoEndDate"))
                        acct.PoEndDate = CommonTools.Utility.ConvertToNullableDateTime(dr["PoEndDate"]);

                    if (dr.Table.Columns.Contains("PoInitialFunds"))
                        acct.PoInitialFunds = CommonTools.Utility.ConvertToNullableDouble(dr["PoInitialFunds"]);

                    if (dr.Table.Columns.Contains("PoRemainingFunds"))
                        acct.PoRemainingFunds = CommonTools.Utility.ConvertToNullableDouble(dr["PoRemainingFunds"]);

                    if (dr.Table.Columns.Contains("Active"))
                        acct.AccountActive = (dr["Active"] == DBNull.Value) ? false : Convert.ToBoolean(dr["Active"]);

                    result.Add(acct);
                }
            }
            return result;
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

        public static string GetFullAccountName(IAccount acct)
        {
            return GetFullAccountName(acct.AccountName, acct.ShortCode, acct.OrgName);
        }

        public static bool GetIsRegularAccountType(int accountTypeId) => accountTypeId == 1;
    }
}
