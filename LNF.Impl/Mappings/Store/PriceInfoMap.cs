using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class PriceInfoMap : ClassMap<PriceInfo>
    {
        internal PriceInfoMap()
        {
            Schema("sselMAS.dbo");
            Table("v_PriceInfo");
            ReadOnly();
            Id(x => x.PriceID);
            Map(x => x.VendorPackageID);
            Map(x => x.ItemID);
            Map(x => x.BaseQMultiplier);
            Map(x => x.PriceBreakQuantity);
            Map(x => x.PackageCost);
            Map(x => x.PackageMarkup);
            Map(x => x.PackagePrice);
            Map(x => x.DateActive);
        }
    }
}
