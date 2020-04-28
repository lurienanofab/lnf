using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class ShippingMethodMap:ClassMap<ShippingMethod>
    {
        internal ShippingMethodMap()
        {
            Schema("IOF.dbo");
            Id(x => x.ShippingMethodID);
            Map(x => x.ShippingMethodName, "ShippingMethod");
        }
    }
}
