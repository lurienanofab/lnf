using System;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IAccountSubsidyManager
    {
        IEnumerable<IAccountSubsidy> GetAccountSubsidy(int? accountId = null);
        IEnumerable<IAccountSubsidy> GetActiveAccountSubsidy(DateTime sd, DateTime ed);
        int AddAccountSubsidy(IAccountSubsidy model);
        bool DisableAccountSubsidy(int accountSubsidyId);
        IAccountSubsidy GetSingleAccountSubsidy(int accountSubsidyId);
    }
}