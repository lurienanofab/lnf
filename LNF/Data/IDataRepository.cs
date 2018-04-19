using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IDataRepository
    {
        IEnumerable<Account> FindActiveAccountsInDateRange(int clientId, DateTime sd, DateTime ed);
    }
}