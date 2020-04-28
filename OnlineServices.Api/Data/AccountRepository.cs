using LNF.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineServices.Api.Data
{
    public class AccountRepository : ApiClient, IAccountRepository
    {
        public IAccount GetAccount(int accountId)
        {
            return Get<Account>("webapi/data/account/{accountId}", UrlSegments(new { accountId }));
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

        public void Delete(int accountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetAccounts(string shortCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> FindClientAccounts(int clientOrgId)
        {
            throw new NotImplementedException();
        }

        public IFundingSource GetFundingSource(int fundingSourceId)
        {
            throw new NotImplementedException();
        }

        public string GetFundingSourceName(IAccount acct)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetNonBillingAccounts()
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

        public ISpecialTopic GetSpecialTopic(int specialTopicId)
        {
            throw new NotImplementedException();
        }

        public string GetSpecialTopicName(IAccount acct)
        {
            throw new NotImplementedException();
        }

        public ITechnicalField GetTechnicalField(int technicalFieldId)
        {
            throw new NotImplementedException();
        }

        public string GetTechnicalFieldName(IAccount acct)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> GetActiveAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IOrg GetOrg(int accountId)
        {
            throw new NotImplementedException();
        }

        public IAccount GetAccount(IChargeType chargeType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccountType> GetAccountTypes()
        {
            throw new NotImplementedException();
        }
    }
}