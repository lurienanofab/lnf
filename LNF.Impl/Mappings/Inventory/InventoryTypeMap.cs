using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class InventoryTypeMap : ClassMap<InventoryType>
    {
        public InventoryTypeMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.InventoryTypeID);
            Map(x => x.InventoryTypeName);
            Map(x => x.Deleted);
            HasMany(x => x.ItemInventoryTypes).KeyColumn("InventoryTypeID");
            HasMany(x => x.CheckOutCategories).KeyColumn("InventoryTypeID");
        }
    }
}
