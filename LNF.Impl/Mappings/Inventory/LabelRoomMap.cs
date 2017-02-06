using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class LabelRoomMap: ClassMap<LabelRoom>
    {
        public LabelRoomMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.LabelRoomID);
            Map(x => x.Slug);
            Map(x => x.RoomName);
        }
    }
}
