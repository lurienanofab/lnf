using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IAccountSubsidyManager : IManager
    {
        IEnumerable<AccountSubsidy> GetActive(DateTime sd, DateTime ed);
    }
}