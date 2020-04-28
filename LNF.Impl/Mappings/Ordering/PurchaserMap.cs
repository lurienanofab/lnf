using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaserMap:ClassMap<Purchaser>
    {
        internal PurchaserMap()
        {
            Schema("IOF.dbo");
            Id(x => x.PurchaserID);
            References(x => x.Client);
            Map(x => x.Active);
            Map(x => x.Deleted);
        }
    }
}
