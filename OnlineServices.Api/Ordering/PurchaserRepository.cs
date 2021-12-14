using LNF.Ordering;
using RestSharp;
using System;

namespace OnlineServices.Api.Ordering
{
    public class PurchaserRepository : ApiClient, IPurchaserRepository
    {
        internal PurchaserRepository(IRestClient rc) : base(rc) { }

        public bool IsPurchaser(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
