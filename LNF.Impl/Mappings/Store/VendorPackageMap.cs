using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    public class VendorPackageMap : ClassMap<VendorPackage>
    {
        public VendorPackageMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.VendorPackageID);
            References(x => x.Vendor);
            References(x => x.Package);
            Map(x => x.VendorPN);
            Map(x => x.Active);
        }
    }
}
