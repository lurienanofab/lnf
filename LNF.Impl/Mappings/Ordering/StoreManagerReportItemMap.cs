using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class StoreManagerReportItemMap : ClassMap<StoreManagerReportItem>
    {
        public StoreManagerReportItemMap()
        {
            ReadOnly();
            Id(x => x.ItemID);
            Map(x => x.Description);
            Map(x => x.VendorID);
            Map(x => x.VendorName);
            Map(x => x.LastOrdered);
            Map(x => x.Unit);
            Map(x => x.UnitPrice);
            Map(x => x.StoreItemID);
            Map(x => x.StoreDescription);
            Map(x => x.StorePackageQuantity, "StorePackageQty");
            Map(x => x.StorePackagePrice);
            Map(x => x.StoreUnitPrice);
        }
    }
}
