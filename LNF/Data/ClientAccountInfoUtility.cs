using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public static class ClientAccountInfoUtility
    {
        public static ClientAccountInfo Find(int clientAccountId)
        {
            return DA.Current.Single<ClientAccountInfo>(clientAccountId);
        }

        public static ClientAccountInfo FindByClientOrg(int clientOrgId, int accountId)
        {
            return DA.Current.Query<ClientAccountInfo>().FirstOrDefault(x => x.ClientOrgID == clientOrgId && x.AccountID == accountId);
        }

        public static ClientAccountInfo FindByClient(int clientId, int accountId)
        {
            return DA.Current.Query<ClientAccountInfo>().FirstOrDefault(x => x.ClientID == clientId && x.AccountID == accountId);
        }
    }
}
