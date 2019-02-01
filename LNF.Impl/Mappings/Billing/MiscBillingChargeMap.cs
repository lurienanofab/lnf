using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class MiscBillingChargeMap : ClassMap<MiscBillingCharge>
    {
        internal MiscBillingChargeMap()
        {
            Table("MiscBillingCharge");
            Id(x => x.ExpID);
            References(x => x.Client);
            References(x => x.Account);
            Map(x => x.SubType, "SUBType");
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