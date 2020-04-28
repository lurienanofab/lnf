using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;
using System.Linq;

namespace LNF.Impl.Ordering
{
    public class PurchaserRepository : RepositoryBase, IPurchaserRepository
    {
        public PurchaserRepository(ISessionManager mgr) : base(mgr) { }

        public bool IsPurchaser(int clientId)
        {
            return Session.Query<Purchaser>().Any(x => x.Client.ClientID == clientId && x.Active && !x.Deleted);
        }
    }
}
