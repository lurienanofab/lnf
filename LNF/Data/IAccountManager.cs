using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public interface IAccountManager : IManager
    {
        IQueryable<Account> Accounts(AccountType item);
        IQueryable<Account> ActiveAccounts();
        IQueryable<ClientAccount> ClientAccounts(Account item);
        IList<Account> ConvertToAccountList(DataTable dt);
        DataTable ConvertToAccountTable(IList<Account> collection);
        void Delete(int accountId);
        IQueryable<Account> FindActiveInDateRange(int clientId, DateTime sd, DateTime ed);
        IQueryable<Account> FindByShortCode(string shortCode);
        FundingSource FundingSource(Account item);
        string FundingSourceName(Account item);
        IQueryable<Account> GetAllNonBillingAccounts();
        string GetChartField(Account item, ChartFieldName field);
        AccountChartFields GetChartFields(Account item);
        string GetDeptRef(Account item, DateTime period);
        void Restore(int accountId);
        SpecialTopic SpecialTopic(Account item);
        string SpecialTopicName(Account item);
        TechnicalField TechnicalField(Account item);
        string TechnicalFieldName(Account item);
    }
}