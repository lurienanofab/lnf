using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Ordering;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Ordering
{
    public class VendorMap : ClassMap<Vendor>
    {
        public VendorMap()
        {
            Schema("IOF.dbo");
            Id(x => x.VendorID);
            Map(x => x.ClientID);
            Map(x => x.VendorName);
            Map(x => x.Address1);
            Map(x => x.Address2);
            Map(x => x.Address3);
            Map(x => x.Contact);
            Map(x => x.Phone);
            Map(x => x.Fax);
            Map(x => x.URL);
            Map(x => x.Email);
            Map(x => x.Active);
        }
    }
}
