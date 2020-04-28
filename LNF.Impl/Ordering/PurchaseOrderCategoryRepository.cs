using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;
using System.Linq;

namespace LNF.Impl.Ordering
{
    public class PurchaseOrderCategoryRepository : RepositoryBase, IPurchaseOrderCategoryRepository
    {
        public PurchaseOrderCategoryRepository(ISessionManager mgr) : base(mgr) { }

        public IPurchaseOrderCategory GetParent(int parentId)
        {
            return Session.Query<PurchaseOrderCategory>().FirstOrDefault(x => x.ParentID == parentId).CreateModel<IPurchaseOrderCategory>();
        }
    }
}
