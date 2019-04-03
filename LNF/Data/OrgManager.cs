using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class OrgManager : ManagerBase, IOrgManager
    {
        public OrgManager(IProvider provider) : base(provider) { }

        public IQueryable<Account> GetAccounts(Org item)
        {
            return Session.Query<Account>().Where(x => x.Org.OrgID == item.OrgID);
        }

        public IQueryable<ClientOrg> GetClientOrgs(Org item)
        {
            return Session.Query<ClientOrg>().Where(x => x.Org.OrgID == item.OrgID);
        }

        public IQueryable<Department> GetDepartments(Org item)
        {
            return Session.Query<Department>().Where(x => x.Org.OrgID == item.OrgID);
        }

        public Org GetPrimaryOrg()
        {
            return Session.Query<Org>().FirstOrDefault(x => x.PrimaryOrg);
        }

        public IList<Org> SelectActive()
        {
            IList<Org> result = Session.Query<Org>().Where(x => x.Active).OrderBy(x => x.OrgName).ToList();
            return result;
        }

        public IQueryable<Org> Orgs(OrgType item)
        {
            return Session.Query<Org>().Where(x => x.OrgType.OrgTypeID == item.OrgTypeID);
        }
    }
}
