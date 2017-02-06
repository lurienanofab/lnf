using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class ClientAccountUtility
    {
        public static IList<ClientAccount> FindClientAccounts(int clientOrgId)
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId).ToList();
        }
    }
}
