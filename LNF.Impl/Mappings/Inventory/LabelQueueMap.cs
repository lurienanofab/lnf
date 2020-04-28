using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class LabelQueueMap : ClassMap<LabelQueue>
    {
        internal LabelQueueMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.LabelQueueID);
            Map(x => x.ItemID);
            Map(x => x.PrivateChemicalID);
            References(x => x.PrintedBy).Column("PrintedByClientID");
            Map(x => x.PrintedDate);
            Map(x => x.LabelType);
            Map(x => x.LabelLocationID);
        }
    }
}
