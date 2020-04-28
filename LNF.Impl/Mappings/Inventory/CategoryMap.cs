using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class CategoryMap : ClassMap<Category>
    {
        internal CategoryMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.CatID);
            Map(x => x.ParentID);
            Map(x => x.HierarchyLevel);
            Map(x => x.CatName);
            Map(x => x.Description);
            Map(x => x.Active);
            Map(x => x.StoreDisplay);
            Map(x => x.RequireLocation);
            HasMany(x => x.Items).KeyColumn("CatID");
            HasMany(x => x.Children).KeyColumn("ParentID");
        }
    }
}
