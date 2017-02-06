using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public static class AccountUtility
    {
        public static IQueryable<Account> ActiveAccounts()
        {
            return DA.Current.Query<Account>().Where(x => x.Active);
        }

        public static IQueryable<Account> FindActiveInDateRange(int clientId, DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(DA.Current.Query<ClientAccount>(), o => o.Record, i => i.ClientAccountID, (outer, inner) => inner);
            var result = join.Where(x => x.ClientOrg.Client.ClientID == clientId).Select(x => x.Account);
            return result;
        }

        public static IQueryable<Account> FindByShortCode(string shortCode)
        {
            return DA.Current.Query<Account>().Where(x => x.ShortCode == shortCode);
        }

        public static IQueryable<Account> GetAllNonBillingAccounts()
        {
            //Account_Select @Action='GetAllNonBillingAccounts'
            int[] nonBillingAccountTypeIds = new int[] { 2, 3 };
            var result = DA.Current.Query<Account>().Where(x => nonBillingAccountTypeIds.Contains(x.AccountType.AccountTypeID));
            return result;
        }

        public static void Restore(int accountId)
        {
            Account acct = DA.Current.Single<Account>(accountId);
            if (acct != null && !acct.Active)
            {
                acct.Enable();
            }
        }

        public static void Delete(int accountId)
        {
            Account acct = DA.Current.Single<Account>(accountId);
            if (acct != null && acct.Active)
            {
                acct.Disable();
            }
        }

        public static IList<Account> ConvertToAccountList(DataTable dt)
        {
            List<Account> result = new List<Account>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Account acct = new Account();
                    if (dr.Table.Columns.Contains("AccountID"))
                        acct.AccountID = Convert.ToInt32(dr["AccountID"]);
                    if (dr.Table.Columns.Contains("OrgID"))
                        acct.Org = (dr["OrgID"] == DBNull.Value) ? null : DA.Current.Single<Org>(Convert.ToInt32(dr["OrgID"]));
                    if (dr.Table.Columns.Contains("Name"))
                        acct.Name = (dr["Name"] == DBNull.Value) ? null : dr["Name"].ToString();
                    if (dr.Table.Columns.Contains("AccountTypeID"))
                        acct.AccountType = (dr["AccountTypeID"] == DBNull.Value) ? null : DA.Current.Single<AccountType>(Convert.ToInt32(dr["AccountTypeID"]));
                    if (dr.Table.Columns.Contains("Number"))
                        acct.Number = (dr["Number"] == DBNull.Value) ? null : dr["Number"].ToString();
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
                        acct.PoEndDate = Utility.ConvertObjectToNullableDateTime(dr["PoEndDate"]);
                    if (dr.Table.Columns.Contains("PoInitialFunds"))
                        acct.PoInitialFunds = Utility.ConvertObjectToNullableDecimal(dr["PoInitialFunds"]);
                    if (dr.Table.Columns.Contains("PoRemainingFunds"))
                        acct.PoRemainingFunds = Utility.ConvertObjectToNullableDecimal(dr["PoRemainingFunds"]);
                    if (dr.Table.Columns.Contains("Active"))
                        acct.Active = (dr["Active"] == DBNull.Value) ? false : Convert.ToBoolean(dr["Active"]);
                    result.Add(acct);
                }
            }
            return result;
        }

        public static DataTable ConvertToAccountTable(IList<Account> collection)
        {
            var dt = new DataTable();
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            collection.OrderBy(x => x.Name).Select(x => dt.Rows.Add(x.AccountID, x.Name)).ToList();
            return dt;
        }
    }
}
