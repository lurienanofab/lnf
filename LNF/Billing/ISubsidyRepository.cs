using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface ISubsidyRepository
    {
        IEnumerable<IAccountSubsidy> GetAccountSubsidy(int? accountId = null);
        IEnumerable<IAccountSubsidy> GetActiveAccountSubsidy(DateTime sd, DateTime ed);
        int AddAccountSubsidy(IAccountSubsidy model);
        bool DisableAccountSubsidy(int accountSubsidyId);
        IAccountSubsidy GetSingleAccountSubsidy(int accountSubsidyId);
        IEnumerable<ITieredSubsidyBilling> PopulateSubsidyBilling(DateTime period, int clientId = 0);
    }
}