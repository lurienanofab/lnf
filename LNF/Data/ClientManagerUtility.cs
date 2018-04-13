using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class ClientManagerUtility
    {
        public static IList<Repository.Data.ClientManager> FindManagers(int clientOrgId)
        {
            return DA.Current.Query<Repository.Data.ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == clientOrgId).ToList();
        }

        public static IList<Repository.Data.ClientManager> FindEmployees(int clientOrgId)
        {
            return DA.Current.Query<Repository.Data.ClientManager>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId).ToList();
        }
    }
}
