using System;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IAccountSubsidyClient
    {
        AccountSubsidyItem AddAccountSubsidy(AccountSubsidyItem model);
        AccountSubsidyItem DisableAccountSubsidy(int accountSubsidyId);
        IEnumerable<AccountSubsidyItem> GetAccountSubsidy(DateTime sd, DateTime ed);
    }
}