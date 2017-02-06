using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Ordering;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Ordering
{
    public class PurchaseOrderCategoryMap : ClassMap<PurchaseOrderCategory>
    {
        public PurchaseOrderCategoryMap()
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
