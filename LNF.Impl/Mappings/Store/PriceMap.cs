using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class PriceMap : ClassMap<Price>
    {
        internal PriceMap()
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
