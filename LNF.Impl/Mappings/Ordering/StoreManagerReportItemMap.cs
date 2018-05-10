using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class StoreManagerReportItemMap : ClassMap<StoreManagerReportItem>
    {
        internal StoreManagerReportItemMap()
        {
            ReadOnly();
            Id(x => x.ItemID);
            Map(x => x.Description);
            Map(x => x.VendorID);
            Map(x => x.VendorName);
            Map(x => x.UnitPrice);
            Map(x => x.Unit);
            Map(x => x.LastOrdered);
            Map(x => x.StoreItemID);
            Map(x => x.StoreDescription);
            Map(x => x.StorePackagePrice);
            Map(x => x.StorePackageQty);
            Map(x => x.StoreUnitPrice);
            Map(x => x.LastPurchased);
        }
    }
}
