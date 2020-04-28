using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class ItemPriceHistoryMap : ClassMap<ItemPriceHistory>
    {
        internal ItemPriceHistoryMap()
        {
            Schema("sselMAS.dbo");
            Table("v_ItemPriceHistory");
            ReadOnly();
            Id(x => x.PriceID);
            Map(x => x.ItemID);
            Map(x => x.PackageID);
            Map(x => x.VendorPackageID);
            Map(x => x.DateActive);
            Map(x => x.BaseQMultiplier);
            Map(x => x.PackageCost);
            Map(x => x.PackageMarkup);
            Map(x => x.PackagePrice);
            Map(x => x.UnitCost);
            Map(x => x.UnitPrice);
        }
    }
}
