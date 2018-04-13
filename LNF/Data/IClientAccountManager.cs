using System.Collections.Generic;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IClientAccountManager : IManager
    {
        IList<ClientAccount> FindClientAccounts(int clientOrgId);
        bool HasDryBox(ClientAccount item);
    }
}