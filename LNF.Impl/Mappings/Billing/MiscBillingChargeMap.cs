using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class MiscBillingChargeMap : ClassMap<MiscBillingCharge>
    {
        internal MiscBillingChargeMap()
        {
            Table("MiscBillingCharge");
            Id(x => x.ExpID);
            Map(x => x.ClientID);
            Map(x => x.AccountID);
            Map(x => x.SUBType);
            Map(x => x.Period);
            Map(x => x.ActDate);
            Map(x => x.Description);
            Map(x => x.Quantity);
            Map(x => x.UnitCost);
            Map(x => x.SubsidyDiscount);
            Map(x => x.Active);
        }
    }
}