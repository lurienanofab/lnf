using LNF.CommonTools;
using LNF.Impl.DataAccess;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class AccountManager : ManagerBase, IAccountManager
    {
        protected ISessionManager SessionManager { get; }

        public AccountManager(IProvider provider, ISessionManager sessionManager) : base(provider)
        {
            SessionManager = sessionManager;
        }

        public IAccount GetAccount(int accountId)
        {
            return Session.Single<Account>(accountId).CreateModel<IAccount>();
        }

        public IEnumerable<IAccount> GetAccounts()
        {
            return Session.Query<Account>().CreateModels<IAccount>();
        }

        public IEnumerable<IAccount> GetAccounts(int accountTypeId)
        {
            return Session.Query<AccountInfo>().Where(x => x.AccountTypeID == accountTypeId).CreateModels<IAccount>();
        }

        public IEnumerable<IAccount> GetActiveAccounts()
        {
            return Session.Query<AccountInfo>().Where(x => x.AccountActive).CreateModels<IAccount>();
        }

        public IEnumerable<IAccount> GetActiveAccounts(int clientId, DateTime sd, DateTime ed)
        {
            Account acct = null;
            ClientAccount ca = null;
            ClientOrg co = null;
            ActiveLog alog1 = null;
            ActiveLog alog2 = null;

            var result = SessionManager.Session.QueryOver(() => acct)
                .JoinEntityAlias(() => ca, () => ca.Account.AccountID == acct.AccountID)
                .JoinEntityAlias(() => co, () => co.ClientOrgID == ca.ClientOrg.ClientOrgID)
                .JoinEntityAlias(() => alog1, () => alog1.Record == ca.ClientAccountID && alog1.TableName == "ClientAccount")
                .JoinEntityAlias(() => alog2, () => alog2.Record == ca.ClientOrg.ClientOrgID && alog2.TableName == "ClientOrg")
                .Where(() =>
                    (co.Client.ClientID == clientId)
                    && (alog1.EnableDate < ed && (alog1.DisableDate == null || alog1.DisableDate > sd))
                    && (alog2.EnableDate < ed && (alog2.DisableDate == null || alog2.DisableDate > sd)))
                .List();

            return result.CreateModels<IAccount>();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int accountId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.AccountID == accountId).CreateModels<IClientAccount>();
        }

        public IList<IAccount> ConvertToAccountList(DataTable dt)
        {
            List<IAccount> result = new List<IAccount>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var acct = new AccountItem();

                    if (dr.Table.Columns.Contains("AccountID"))
                        acct.AccountID = Convert.ToInt32(dr["AccountID"]);

                    if (dr.Table.Columns.Contains("OrgID"))
                    {
                        var org = Session.Single<Org>(Convert.ToInt32(dr["OrgID"]));
                        acct.OrgID = org.OrgID;
                        acct.OrgName = org.OrgName;
                    }

                    if (dr.Table.Columns.Contains("Name"))
                        acct.AccountName = (dr["Name"] == DBNull.Value) ? null : dr["Name"].ToString();

                    if (dr.Table.Columns.Contains("AccountTypeID"))
                    {
                        var acctType = Session.Single<AccountType>(Convert.ToInt32(dr["AccountTypeID"]));
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
                        acct.PoEndDate = Utility.ConvertToNullableDateTime(dr["PoEndDate"]);

                    if (dr.Table.Columns.Contains("PoInitialFunds"))
                        acct.PoInitialFunds = Utility.ConvertToNullableDouble(dr["PoInitialFunds"]);

                    if (dr.Table.Columns.Contains("PoRemainingFunds"))
                        acct.PoRemainingFunds = Utility.ConvertToNullableDouble(dr["PoRemainingFunds"]);

                    if (dr.Table.Columns.Contains("Active"))
                        acct.AccountActive = (dr["Active"] == DBNull.Value) ? false : Convert.ToBoolean(dr["Active"]);

                    result.Add(acct);
                }
            }
            return result;
        }

        public DataTable ConvertToAccountTable(IList<IAccount> accounts)
        {
            var dt = new DataTable();
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Name", typeof(string));

            foreach (var a in accounts.OrderBy(x => x.AccountName))
                dt.Rows.Add(a.AccountID, a.AccountName);

            return dt;
        }

        public bool Delete(int accountId)
        {
            Account acct = Session.Single<Account>(accountId);
            if (acct != null && acct.Active)
            {
                Provider.Data.ActiveLog.Disable("Account", accountId);
                return true;
            }
            return false;
        }

        public IEnumerable<IAccount> FindByShortCode(string shortCode)
        {
            return Session.Query<AccountInfo>().Where(x => x.ShortCode == shortCode).CreateModels<IAccount>();
        }

        public IEnumerable<IClientAccount> FindClientAccounts(int clientOrgId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientOrgID == clientOrgId).CreateModels<IClientAccount>();
        }

        public IFundingSource FundingSource(int fundingSourceId)
        {
            return Session.Single<FundingSource>(fundingSourceId).CreateModel<IFundingSource>();
        }

        public string FundingSourceName(IAccount acct)
        {
            var fs = FundingSource(acct.FundingSourceID);

            if (fs != null)
                return fs.FundingSourceName;
            else
                return string.Empty;
        }

        public IEnumerable<IAccount> GetAllNonBillingAccounts()
        {
            //Account_Select @Action='GetAllNonBillingAccounts'
            int[] nonBillingAccountTypeIds = new int[] { 2, 3 };
            var result = Session.Query<AccountInfo>().Where(x => nonBillingAccountTypeIds.Contains(x.AccountTypeID)).CreateModels<IAccount>();
            return result;
        }

        public string GetChartField(IAccount acct, ChartFieldName field)
        {
            IAccountChartFields fields = GetChartFields(acct);
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

        public IAccountChartFields GetChartFields(IAccount acct)
        {
            return new AccountChartFields(acct);
        }

        public string GetDeptRef(IAccount acct, DateTime period)
        {
            //this is the Project chart field based on OrgRecharge or Org.OrgType.Account

            var allOrgRecharge = Session.Query<OrgRecharge>();

            var orgRecharge = allOrgRecharge.Where(x => x.Account.AccountID == acct.AccountID && x.EnableDate < period.AddMonths(1) && (x.DisableDate == null || x.DisableDate > period)).OrderBy(x => x.OrgRechargeID).LastOrDefault();

            if (orgRecharge != null)
                return orgRecharge.Account.GetChartFields().Project;
            else
                return GetChartFields(acct).Project;
        }

        public void Restore(int accountId)
        {
            Account acct = Session.Single<Account>(accountId);
            if (acct != null && !acct.Active)
            {
                Provider.Data.ActiveLog.Enable("Account", acct.AccountID);
            }
        }

        public ISpecialTopic SpecialTopic(int specialTopicId)
        {
            return Session.Single<SpecialTopic>(specialTopicId).CreateModel<ISpecialTopic>();
        }

        public string SpecialTopicName(IAccount acct)
        {
            var st = SpecialTopic(acct.SpecialTopicID);

            if (st != null)
                return st.SpecialTopicName;
            else
                return string.Empty;
        }

        public ITechnicalField TechnicalField(int technicalFieldId)
        {
            return Session.Single<TechnicalField>(technicalFieldId).CreateModel<ITechnicalField>();
        }

        public string TechnicalFieldName(IAccount acct)
        {
            var tf = TechnicalField(acct.TechnicalFieldID);

            if (tf != null)
                return tf.TechnicalFieldName;
            else
                return string.Empty;
        }
    }
}
