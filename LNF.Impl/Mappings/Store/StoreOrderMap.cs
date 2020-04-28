using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class StoreOrderMap:ClassMap<StoreOrder>
    {
        internal StoreOrderMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.SOID);
            References(x => x.Client);
            References(x => x.Account);
            Map(x => x.CreationDate);
            Map(x => x.Status);
            Map(x => x.StatusChangeDate);
            Map(x => x.InventoryLocationID);
        }
    }
}
