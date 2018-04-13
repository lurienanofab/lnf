using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class ClientAccountManager : ManagerBase, IClientAccountManager
    {
        public ClientAccountManager(ISession session) : base(session) { }

        public bool HasDryBox(ClientAccount item)
        {
            IList<DryBoxAssignment> query = Session.Query<DryBoxAssignment>().Where(x => x.ClientAccount.ClientAccountID == item.ClientAccountID).ToList();
            DryBoxAssignment dba = query.FirstOrDefault(x => x.GetStatus() == DryBoxAssignmentStatus.Active);
            return dba != null;
        }

        public IList<ClientAccount> FindClientAccounts(int clientOrgId)
        {
            return Session.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId).ToList();
        }
    }
}
