using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace LNF.Ordering
{
    public static class VendorUtility
    {
        public static Vendor GetVendor(string id)
        {
            return DA.Current.Single<Vendor>(Convert.ToInt32(id));
        }

        public static Vendor GetVendor(int id)
        {
            return DA.Current.Single<Vendor>(id);
        }

        public static IEnumerable<Vendor> SelectVendorList(int clientId, bool? active = true)
        {
            IList<Vendor> query = null;
            if (active == null)
                query = DA.Current.Query<Vendor>().Where(x => x.ClientID == clientId).ToList();
            else
                query = DA.Current.Query<Vendor>().Where(x => x.ClientID == clientId && x.Active == active.Value).ToList();
            return query.OrderBy(x => x.VendorName);
        }

        public static Vendor CopyData(Client toClient, Vendor fromVendor)
        {
            Vendor v = new Vendor()
            {
                Active = true,
                Address1 = fromVendor.Address1,
                Address2 = fromVendor.Address2,
                Address3 = fromVendor.Address3,
                ClientID = toClient.ClientID,
                Contact = fromVendor.Contact,
                Email = fromVendor.Email,
                Fax = fromVendor.Fax,
                Phone = fromVendor.Phone,
                URL = fromVendor.URL,
                VendorName = fromVendor.VendorName
            };

            DA.Current.Insert(v);

            //returns the newly created vendor
            return v;
        }

        public static DataTable GetClientsWithVendors()
        {
            using (var adap = DA.Current.GetAdapter())
            {
                adap.AddParameter("@Action", "GetClientsWithVendors");
                return adap.FillDataTable("IOF.dbo.spVendor_Select");
            }
        }
    }
}
