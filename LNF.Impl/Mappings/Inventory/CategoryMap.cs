using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.CatID);
            References(x => x.Parent, "ParentID");
            Map(x => x.HierarchyLevel);
            Map(x => x.CatName);
            Map(x => x.Description);
            Map(x => x.Active);
            Map(x => x.StoreDisplay);
            HasMany(x => x.Items).KeyColumn("CatID");
            HasMany(x => x.Children).KeyColumn("ParentID");
            Map(x => x.RequireLocation);
        }
    }
}
