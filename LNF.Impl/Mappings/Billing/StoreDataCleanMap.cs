using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class StoreDataCleanMap : ClassMap<StoreDataClean>
    {
        internal StoreDataCleanMap()
        {
            Schema("sselData.dbo");
            Id(x => x.StoreDataID);
            References(x => x.Client);
            References(x => x.Item);
            Map(x => x.OrderDate);
            References(x => x.Account);
            Map(x => x.Quantity);
            Map(x => x.UnitCost);
            References(x => x.Category);
            Map(x => x.RechargeItem);
            Map(x => x.StatusChangeDate);
        }
    }
}
