using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class StoreBillingMap : ClassMap<StoreBilling>
    {
        internal StoreBillingMap()
        {
            Schema("sselData.dbo");
            Table("StoreBilling");
            Id(x => x.StoreBillingID);
            Map(x => x.Period);
            Map(x => x.ClientID);
            Map(x => x.AccountID);
            Map(x => x.ChargeTypeID);
            Map(x => x.ItemID);
            Map(x => x.Quantity);
            Map(x => x.UnitCost);
            Map(x => x.CategoryID);
            Map(x => x.CostMultiplier);
            Map(x => x.LineCost);
            Map(x => x.StatusChangeDate);
        }
    }
}
