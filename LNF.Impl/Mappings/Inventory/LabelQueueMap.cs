using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class LabelQueueMap : ClassMap<LabelQueue>
    {
        public LabelQueueMap()
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
