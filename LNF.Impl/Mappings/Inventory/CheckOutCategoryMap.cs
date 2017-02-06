using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class CheckOutCategoryMap : ClassMap<CheckOutCategory>
    {
        public CheckOutCategoryMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.CheckOutCategoryID);
            References(x => x.InventoryType);
            Map(x => x.CategoryName);
            Map(x => x.Deleted);
        }
    }
}
