using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class MiscBillingChargeItemMap : ClassMap<MiscBillingChargeItem>
    {
        internal MiscBillingChargeItemMap()
        {
            ReadOnly();
            Id(x => x.ExpID);
            Map(x => x.ClientID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.AccountID);
            Map(x => x.AccountName);
            Map(x => x.ShortCode);
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
