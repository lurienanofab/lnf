using LNF.Data;
using System.Collections.Generic;

namespace LNF.Ordering
{
    public static class Vendors
    {
        public static IVendor GetVendor(int vendorId)
        {
            return ServiceProvider.Current.Ordering.Vendor.GetVendor(vendorId);
        }

        public static IEnumerable<IVendor> SelectVendorList(int clientId, bool? active = true)
        {
            if (active == null)
                return ServiceProvider.Current.Ordering.Vendor.GetVendors(clientId);
            else
                return ServiceProvider.Current.Ordering.Vendor.GetActiveVendors(clientId);
        }

        public static IVendor CopyData(int toClientId, IVendor fromVendor)
        {
            return ServiceProvider.Current.Ordering.Vendor.AddVendor(
                clientId: toClientId,
                vendorName: fromVendor.VendorName,
                address1: fromVendor.Address1,
                address2: fromVendor.Address2,
                address3: fromVendor.Address3,
                contact: fromVendor.Contact,
                phone: fromVendor.Phone,
                fax: fromVendor.Fax,
                url: fromVendor.URL,
                email: fromVendor.Email
            );
        }

        public static IEnumerable<ClientListItem> GetClientsWithVendors() => ServiceProvider.Current.Ordering.Vendor.GetClientsWithVendors(); 
    }
}
