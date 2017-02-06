using LNF.Repository;
using LNF.Repository.Data;
using System.Linq;

namespace LNF.Data
{
    public static class ClientOrgInfoUtility
    {
        public static ClientOrgInfo Find(int clientOrgId)
        {
            return DA.Current.Single<ClientOrgInfo>(clientOrgId);
        }

        public static ClientOrgInfo Find(int clientId, int rank)
        {
            return DA.Current.Query<ClientOrgInfo>().FirstOrDefault(x => x.ClientID == clientId && x.EmailRank == rank);
        }
    }
}
