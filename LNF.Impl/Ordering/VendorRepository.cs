using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Ordering
{
    public class VendorRepository : RepositoryBase, IVendorRepository
    {
        public VendorRepository(ISessionManager mgr) : base(mgr) { }

        public IVendor GetVendor(int vendorId)
        {
            return Session.Get<Vendor>(vendorId).CreateModel<IVendor>();
        }

        public IEnumerable<IVendor> GetVendors(int clientId)
        {
            return Session.Query<Vendor>().Where(x => x.ClientID == clientId).CreateModels<IVendor>();
        }

        public IEnumerable<IVendor> GetActiveVendors(int clientId)
        {
            return Session.Query<Vendor>().Where(x => x.ClientID == clientId && x.Active).CreateModels<IVendor>();
        }

        public IEnumerable<IPurchaseOrderItem> GetItems(int vendorId)
        {
            var vendor = Session.Get<Vendor>(vendorId);

            if (vendor == null)
                throw new ItemNotFoundException<Vendor>(x => x.VendorID, vendorId);

            return vendor.Items.CreateModels<IPurchaseOrderItem>();
        }

        public IVendor AddVendor(int clientId, string vendorName, string address1, string address2, string address3, string contact, string phone, string fax, string url, string email)
        {
            Vendor v = new Vendor()
            {
                ClientID = clientId,
                VendorName = vendorName,
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                Contact = contact,
                Phone = phone,
                Fax = fax,
                URL = url,
                Email = email,
                Active = true
            };

            Session.Save(v);

            //returns the newly created vendor
            return v.CreateModel<IVendor>();
        }

        public IEnumerable<ClientListItem> GetClientsWithVendors()
        {
            var table = Session.CreateSQLQuery("EXEC IOF.dbo.spVendor_Select @Action = 'GetClientsWithVendors'").FillTable();

            var result = table.Select(x => new ClientListItem
            {
                ClientID = (int)x["ClientID"],
                DisplayName = (string)x["DisplayName"]
            }).ToList();

            return result;
        }
    }
}
