using System.Linq;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IClientInfoManager : IManager
    {
        IQueryable<ClientAccountInfo> ClientAccounts(ClientInfo item);
        IQueryable<ClientOrgInfo> ClientOrgs(ClientInfo item);
        ClientInfo Find(int clientId);
        ClientInfo FindByClientOrgID(int clientOrgId);
        Client GetClient(ClientInfo item);
    }
}