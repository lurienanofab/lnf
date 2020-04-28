using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class LabelLocationMap: ClassMap<LabelLocation>
    {
        internal LabelLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.LabelLocationID);
            References(x => x.LabelRoom);
            Map(x => x.LocationName);
        }
    }
}
