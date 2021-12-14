using LNF.Data;
using LNF.Ordering;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Ordering
{
    public class VendorRepository : ApiClient, IVendorRepository
    {
        internal VendorRepository(IRestClient rc) : base(rc) { }

        public IVendor AddVendor(int clientId, string vendorName, string address1, string address2, string address3, string contact, string phone, string fax, string url, string email)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVendor> GetActiveVendors(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ClientListItem> GetClientsWithVendors()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPurchaseOrderItem> GetItems(int vendorId)
        {
            throw new NotImplementedException();
        }

        public IVendor GetVendor(int vendorId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVendor> GetVendors(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
