using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Ordering;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Ordering
{
    public class ShippingMethodMap:ClassMap<ShippingMethod>
    {
        public ShippingMethodMap()
        {
            Schema("IOF.dbo");
            Id(x => x.ShippingMethodID);
            Map(x => x.ShippingMethodName, "ShippingMethod");
        }
    }
}
