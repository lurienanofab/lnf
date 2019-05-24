using LNF.Models.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineServices.Api.Data
{
    public class AccountManager : ApiClient, IAccountManager
    {
        public IAccount GetAccount(int accountId)
        {
            return Get<AccountItem>("webapi/data/account/{accountId}", UrlSegments(new { accountId }));
        }

        public IEnumerable<IAccount> GetAccounts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetAccounts(int accountTypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetActiveAccounts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetActiveAccounts(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int accountId)
        {
            throw new NotImplementedException();
        }

        public IList<IAccount> ConvertToAccountList(DataTable dt)
        {
            throw new NotImplementedException();
        }

        public DataTable ConvertToAccountTable(IList<IAccount> accounts)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int accountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> FindByShortCode(string shortCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> FindClientAccounts(int clientOrgId)
        {
            throw new NotImplementedException();
        }

        public IFundingSource FundingSource(int fundingSourceId)
        {
            throw new NotImplementedException();
        }

        public string FundingSourceName(IAccount acct)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetAllNonBillingAccounts()
        {
            throw new NotImplementedException();
        }

        public string GetChartField(IAccount acct, ChartFieldName field)
        {
            throw new NotImplementedException();
        }

        public IAccountChartFields GetChartFields(IAccount acct)
        {
            throw new NotImplementedException();
        }

        public string GetDeptRef(IAccount acct, DateTime period)
        {
            throw new NotImplementedException();
        }

        public void Restore(int accountId)
        {
            throw new NotImplementedException();
        }

        public ISpecialTopic SpecialTopic(int specialTopicId)
        {
            throw new NotImplementedException();
        }

        public string SpecialTopicName(IAccount acct)
        {
            throw new NotImplementedException();
        }

        public ITechnicalField TechnicalField(int technicalFieldId)
        {
            throw new NotImplementedException();
        }

        public string TechnicalFieldName(IAccount acct)
        {
            throw new NotImplementedException();
        }
    }
}