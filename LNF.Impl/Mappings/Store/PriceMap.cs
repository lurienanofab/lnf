using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    public class PriceMap : ClassMap<Price>
    {
        public PriceMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.PriceID);
            References(x => x.VendorPackage);
            Map(x => x.PriceBreakQuantity);
            Map(x => x.PackageCost);
            Map(x => x.PackageMarkup);
            Map(x => x.PackagePrice);
            Map(x => x.DateActive);
        }
    }
}
