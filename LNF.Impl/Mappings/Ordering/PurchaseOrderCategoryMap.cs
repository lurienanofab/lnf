using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaseOrderCategoryMap : ClassMap<PurchaseOrderCategory>
    {
        internal PurchaseOrderCategoryMap()
        {
            Schema("IOF.dbo");
            Table("Category");
            Id(x => x.CatID);
            Map(x => x.CatName);
            Map(x => x.ParentID);
            Map(x => x.Active);
            Map(x => x.CatNo);
        }
    }
}
