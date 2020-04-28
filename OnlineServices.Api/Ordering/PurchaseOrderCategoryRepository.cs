using LNF.Ordering;
using System;

namespace OnlineServices.Api.Ordering
{
    public class PurchaseOrderCategoryRepository : ApiClient, IPurchaseOrderCategoryRepository
    {
        public IPurchaseOrderCategory GetParent(int parentId)
        {
            throw new NotImplementedException();
        }
    }
}
