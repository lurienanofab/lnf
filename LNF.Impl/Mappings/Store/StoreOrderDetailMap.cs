using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class StoreOrderDetailMap : ClassMap<StoreOrderDetail>
    {
        internal StoreOrderDetailMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.SODID);
            References(x => x.StoreOrder, "SOID");
            References(x => x.Item);
            Map(x => x.Quantity);
            Map(x => x.PriceID);
        }
    }
}
