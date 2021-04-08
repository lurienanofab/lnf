using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {
        public AccountRepository(ISessionManager mgr) : base(mgr) { }

        public IAccount GetAccount(int accountId)
        {
            return Session.Get<AccountInfo>(accountId);
        }

        public IEnumerable<IAccount> GetAccounts()
        {
            return Session.Query<AccountInfo>().ToList();
        }

        public IEnumerable<IAccount> GetAccounts(string shortCode)
        {
            return Session.Query<AccountInfo>().Where(x => x.ShortCode == shortCode).CreateModels<IAccount>();
        }

        public IEnumerable<IAccount> GetAccounts(int accountTypeId)
        {
            return Session.Query<AccountInfo>().Where(x => x.AccountTypeID == accountTypeId).CreateModels<IAccount>();
        }

        public IEnumerable<IAccount> GetActiveAccounts()
        {
            return Session.Query<AccountInfo>().Where(x => x.AccountActive).CreateModels<IAccount>();
        }

        public IEnumerable<IAccountType> GetAccountTypes()
        {
            return Session.Query<AccountType>().CreateModels<IAccountType>();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int accountId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.AccountID == accountId).CreateModels<IClientAccount>();
        }

        public IOrg GetOrg(int accountId)
        {
            var acct = Session.Get<Account>(accountId);

            if (acct == null)
                throw new ItemNotFoundException<Account>(x => x.AccountID, accountId);

            return acct.Org.CreateModel<IOrg>();
        }

        public IEnumerable<IAccount> GetNonBillingAccounts()
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

        public void Delete(int accountId)
        {
            Session.Disable(Session.Get<Account>(accountId));
        }

        public void Restore(int accountId)
        {
            Session.Enable(Session.Get<Account>(accountId));
        }

        public IFundingSource GetFundingSource(int fundingSourceId)
        {
            return Session.Get<FundingSource>(fundingSourceId).CreateModel<IFundingSource>();
        }

        public string GetFundingSourceName(IAccount acct)
        {
            var fs = GetFundingSource(acct.FundingSourceID);

            if (fs != null)
                return fs.FundingSourceName;
            else
                return string.Empty;
        }

        public ISpecialTopic GetSpecialTopic(int specialTopicId)
        {
            return Session.Get<SpecialTopic>(specialTopicId).CreateModel<ISpecialTopic>();
        }

        public string GetSpecialTopicName(IAccount acct)
        {
            var st = GetSpecialTopic(acct.SpecialTopicID);

            if (st != null)
                return st.SpecialTopicName;
            else
                return string.Empty;
        }

        public ITechnicalField GetTechnicalField(int technicalFieldId)
        {
            return Session.Get<TechnicalField>(technicalFieldId).CreateModel<ITechnicalField>();
        }

        public string GetTechnicalFieldName(IAccount acct)
        {
            var tf = GetTechnicalField(acct.TechnicalFieldID);

            if (tf != null)
                return tf.TechnicalFieldName;
            else
                return string.Empty;
        }
    }
}
