using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public interface IOrgManager : IManager
    {
        IQueryable<Account> GetAccounts(Org item);
        IQueryable<ClientOrg> GetClientOrgs(Org item);
        IQueryable<Department> GetDepartments(Org item);
        Org GetPrimaryOrg();
        IQueryable<Org> Orgs(OrgType item);
        IList<Org> SelectActive();
    }
}