using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class StoreDataMap : ClassMap<StoreData>
    {
        internal StoreDataMap()
        {
            Schema("sselData.dbo");
            Id(x => x.StoreDataID);
            Map(x => x.Period);
            Map(x => x.ClientID);
            Map(x => x.ItemID);
            Map(x => x.OrderDate);
            Map(x => x.AccountID);
            Map(x => x.Quantity);
            Map(x => x.UnitCost);
            Map(x => x.CategoryID);
            Map(x => x.StatusChangeDate);
        }
    }
}
