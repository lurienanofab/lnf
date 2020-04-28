using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IAccountRepository
    {
        IAccount GetAccount(int accountId);
        IEnumerable<IAccount> GetAccounts();
        IEnumerable<IAccount> GetAccounts(string shortCode);
        IEnumerable<IAccount> GetAccounts(int accountTypeId);
        IEnumerable<IAccount> GetActiveAccounts();
        IEnumerable<IAccountType> GetAccountTypes();
        IEnumerable<IClientAccount> GetClientAccounts(int accountId);
        IOrg GetOrg(int accountId);
        IEnumerable<IAccount> GetNonBillingAccounts();
        string GetChartField(IAccount acct, ChartFieldName field);
        IAccountChartFields GetChartFields(IAccount acct);
        string GetDeptRef(IAccount acct, DateTime period);
        void Delete(int accountId);
        void Restore(int accountId);
        IFundingSource GetFundingSource(int fundingSourceId);
        string GetFundingSourceName(IAccount acct);
        ISpecialTopic GetSpecialTopic(int specialTopicId);
        string GetSpecialTopicName(IAccount acct);
        ITechnicalField GetTechnicalField(int technicalFieldId);
        string GetTechnicalFieldName(IAccount acct);
    }
}
