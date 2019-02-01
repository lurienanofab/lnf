using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal abstract class RoomBillingClassMap<T> : ClassMap<T> where T : IRoomBilling
    {
        internal RoomBillingClassMap()
        {
            Schema("sselData.dbo");
            Map(x => x.Period);
            Map(x => x.ClientID);
            Map(x => x.RoomID);
            Map(x => x.AccountID);
            Map(x => x.ChargeTypeID);
            Map(x => x.BillingTypeID);
            Map(x => x.OrgID);
            Map(x => x.ChargeDays);
            Map(x => x.PhysicalDays);
            Map(x => x.AccountDays);
            Map(x => x.Entries);
            Map(x => x.Hours);
            Map(x => x.IsDefault);
            Map(x => x.RoomRate);
            Map(x => x.EntryRate);
            Map(x => x.MonthlyRoomCharge);
            Map(x => x.RoomCharge).ReadOnly();
            Map(x => x.EntryCharge).ReadOnly();
        }
    }

    internal class RoomBillingMap : RoomBillingClassMap<RoomBilling>
    {
        internal RoomBillingMap()
        {
            Table("RoomApportionmentInDaysMonthly");
            Id(x => x.RoomBillingID, "AppID");
            Map(x => x.SubsidyDiscount);
        }
    }

    internal class RoomBillingTempMap : RoomBillingClassMap<RoomBillingTemp>
    {
        internal RoomBillingTempMap()
        {
            Table("RoomBillingTemp");
            Id(x => x.RoomBillingID);
        }
    }
}
