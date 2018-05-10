using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public class AccountManager : ManagerBase, IAccountManager
    {
        protected IActiveDataItemManager ActiveDataItemManager { get; }
        protected IChargeTypeManager ChargeTypeManager { get; }

        public AccountManager(ISession session, IActiveDataItemManager activeDataItemManager, IChargeTypeManager chargeTypeManager) : base(session)
        {
            ActiveDataItemManager = activeDataItemManager;
            ChargeTypeManager = chargeTypeManager;
        }

        public IQueryable<ClientAccount> ClientAccounts(Account item)
        {
            return Session.Query<ClientAccount>().Where(x => x.Account == item);
        }

        public FundingSource FundingSource(Account item)
        {
            return Session.Single<FundingSource>(item.FundingSourceID);
        }

        public string FundingSourceName(Account item)
        {
            var fs = FundingSource(item);

            if (fs != null)
                return fs.FundingSourceName;
            else
                return string.Empty;
        }

        public TechnicalField TechnicalField(Account item)
        {
            return Session.Single<TechnicalField>(item.TechnicalFieldID);
        }

        public string TechnicalFieldName(Account item)
        {
            var tf = TechnicalField(item);

            if (tf != null)
                return tf.TechnicalFieldName;
            else
                return string.Empty;
        }

        public SpecialTopic SpecialTopic(Account item)
        {
            return Session.Single<SpecialTopic>(item.SpecialTopicID);
        }

        public string SpecialTopicName(Account item)
        {
            var st = SpecialTopic(item);

            if (st != null)
                return st.SpecialTopicName;
            else
                return string.Empty;
        }

        public string GetDeptRef(Account item, DateTime period)
        {
            //this is the Project chart field based on OrgRecharge or Org.OrgType.Account

            var allOrgRecharge = Session.Query<OrgRecharge>();

            var orgRecharge = allOrgRecharge.Where(x => x.Account.AccountID == item.AccountID && x.EnableDate < period.AddMonths(1) && (x.DisableDate == null || x.DisableDate > period)).OrderBy(x => x.OrgRechargeID).LastOrDefault();

            if (orgRecharge != null)
                return orgRecharge.Account.GetChartFields().Project;
            else
                return ChargeTypeManager.GetAccount(item.Org.OrgType.ChargeType).GetChartFields().Project;
        }

        public IQueryable<Account> ActiveAccounts()
        {
            return Session.Query<Account>().Where(x => x.Active);
        }

        public IQueryable<Account> FindByShortCode(string shortCode)
        {
            return Session.Query<Account>().Where(x => x.ShortCode == shortCode);
        }

        public IQueryable<Account> GetAllNonBillingAccounts()
        {
            //Account_Select @Action='GetAllNonBillingAccounts'
            int[] nonBillingAccountTypeIds = new int[] { 2, 3 };
            var result = Session.Query<Account>().Where(x => nonBillingAccountTypeIds.Contains(x.AccountType.AccountTypeID));
            return result;
        }

        public void Restore(int accountId)
        {
            Account acct = Session.Single<Account>(accountId);
            if (acct != null && !acct.Active)
            {
                ActiveDataItemManager.Enable(acct);
            }
        }

        public void Delete(int accountId)
        {
            Account acct = Session.Single<Account>(accountId);
            if (acct != null && acct.Active)
            {
                ActiveDataItemManager.Disable(acct);
            }
        }

        public IList<Account> ConvertToAccountList(DataTable dt)
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
                        acct.Org = (dr["OrgID"] == DBNull.Value) ? null : Session.Single<Org>(Convert.ToInt32(dr["OrgID"]));
                    if (dr.Table.Columns.Contains("Name"))
                        acct.Name = (dr["Name"] == DBNull.Value) ? null : dr["Name"].ToString();
                    if (dr.Table.Columns.Contains("AccountTypeID"))
                        acct.AccountType = (dr["AccountTypeID"] == DBNull.Value) ? null : Session.Single<AccountType>(Convert.ToInt32(dr["AccountTypeID"]));
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

        public IQueryable<Account> Accounts(AccountType item)
        {
            return Session.Query<Account>().Where(x => x.AccountType.AccountTypeID == item.AccountTypeID);
        }

        public string GetChartField(Account item, ChartFieldName field)
        {
            AccountChartFields fields = item.GetChartFields();
            switch (field)
            {
                case ChartFieldName.Account:
                    return fields.Account;
                case ChartFieldName.Fund:
                    return fields.Fund;
                case ChartFieldName.Department:
                    return fields.Department;
                case ChartFieldName.Program:
                    return fields.Program;
                case ChartFieldName.Class:
                    return fields.Class;
                case ChartFieldName.Project:
                    return fields.Project;
                default:
                    return fields.ShortCode;
            }
        }

        public AccountChartFields GetChartFields(Account item)
        {
            return new AccountChartFields(item);
        }

        public DataTable ConvertToAccountTable(IList<Account> accounts)
        {
            var dt = new DataTable();
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Name", typeof(string));

            foreach (var a in accounts.OrderBy(x => x.Name))
                dt.Rows.Add(a.AccountID, a.Name);

            return dt;
        }

        public IQueryable<ClientAccount> FindClientAccounts(int clientOrgId)
        {
            return Session.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId);
        }
    }
}
