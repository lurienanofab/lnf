using LNF.Ordering;
using System;

namespace OnlineServices.Api.Ordering
{
    public class PurchaserRepository : ApiClient, IPurchaserRepository
    {
        public bool IsPurchaser(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
