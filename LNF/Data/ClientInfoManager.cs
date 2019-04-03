using LNF.Repository;
using LNF.Repository.Data;
using System.Linq;

namespace LNF.Data
{
    public class ClientInfoManager : ManagerBase, IClientInfoManager
    {
        public ClientInfoManager(IProvider provider) : base(provider) { }

        public IQueryable<ClientOrgInfo> ClientOrgs(ClientInfo item)
        {
            return Session.Query<ClientOrgInfo>().Where(x => x.ClientID == item.ClientID);
        }

        public IQueryable<ClientAccountInfo> ClientAccounts(ClientInfo item)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientID == item.ClientID);
        }

        public Client GetClient(ClientInfo item)
        {
            return Session.Single<Client>(item.ClientID);
        }

        public ClientInfo Find(int clientId)
        {
            return Session.Single<ClientInfo>(clientId);
        }

        public ClientInfo FindByClientOrgID(int clientOrgId)
        {
            return Session.Query<ClientInfo>().FirstOrDefault(x => x.ClientOrgID == clientOrgId);
        }
    }

}
