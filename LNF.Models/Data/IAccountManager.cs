using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Models.Data
{
    public interface IAccountManager
    {
        IAccount GetAccount(int accountId);
        IEnumerable<IAccount> GetAccounts();
        IEnumerable<IAccount> GetAccounts(int accountTypeId);
        IEnumerable<IAccount> GetActiveAccounts();
        IEnumerable<IAccount> GetActiveAccounts(int clientId, DateTime sd, DateTime ed);
        IEnumerable<IClientAccount> GetClientAccounts(int accountId);
        IList<IAccount> ConvertToAccountList(DataTable dt);
        DataTable ConvertToAccountTable(IList<IAccount> accounts);
        bool Delete(int accountId);
        IEnumerable<IAccount> FindByShortCode(string shortCode);
        IFundingSource FundingSource(int fundingSourceId);
        string FundingSourceName(IAccount acct);
        IEnumerable<IAccount> GetAllNonBillingAccounts();
        string GetChartField(IAccount acct, ChartFieldName field);
        IAccountChartFields GetChartFields(IAccount acct);
        string GetDeptRef(IAccount acct, DateTime period);
        void Restore(int accountId);
        ISpecialTopic SpecialTopic(int specialTopicId);
        string SpecialTopicName(IAccount acct);
        ITechnicalField TechnicalField(int technicalFieldId);
        string TechnicalFieldName(IAccount acct);
        IEnumerable<IClientAccount> FindClientAccounts(int clientOrgId);
    }
}
