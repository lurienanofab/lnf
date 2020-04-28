using LNF.Data;
using System.Collections.Generic;

namespace LNF.Ordering
{
    public interface IVendorRepository
    {
        IVendor GetVendor(int vendorId);
        IEnumerable<IVendor> GetVendors(int clientId);
        IEnumerable<IVendor> GetActiveVendors(int clientId);
        IEnumerable<IPurchaseOrderItem> GetItems(int vendorId);
        IVendor AddVendor(int clientId, string vendorName, string address1, string address2, string address3, string contact, string phone, string fax, string url, string email);
        IEnumerable<ClientListItem> GetClientsWithVendors();
    }
}
