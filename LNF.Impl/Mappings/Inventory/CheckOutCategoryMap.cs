using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class CheckOutCategoryMap : ClassMap<CheckOutCategory>
    {
        internal CheckOutCategoryMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.CheckOutCategoryID);
            References(x => x.InventoryType);
            Map(x => x.CategoryName);
            Map(x => x.Deleted);
        }
    }
}
