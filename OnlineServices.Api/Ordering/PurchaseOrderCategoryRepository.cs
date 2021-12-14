using LNF.Ordering;
using RestSharp;
using System;

namespace OnlineServices.Api.Ordering
{
    public class PurchaseOrderCategoryRepository : ApiClient, IPurchaseOrderCategoryRepository
    {
        internal PurchaseOrderCategoryRepository(IRestClient rc) : base(rc) { }

        public IPurchaseOrderCategory GetParent(int parentId)
        {
            throw new NotImplementedException();
        }
    }
}
