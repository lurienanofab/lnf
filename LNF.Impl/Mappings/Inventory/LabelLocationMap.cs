using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class LabelLocationMap: ClassMap<LabelLocation>
    {
        public LabelLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.LabelLocationID);
            References(x => x.LabelRoom);
            Map(x => x.LocationName);
        }
    }
}
