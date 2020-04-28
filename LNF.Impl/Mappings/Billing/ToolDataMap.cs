using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ToolDataMap : ClassMap<ToolData>
    {
        internal ToolDataMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ToolDataID);
            Map(x => x.Period);
            Map(x => x.ClientID);
            Map(x => x.ResourceID);
            Map(x => x.RoomID);
            Map(x => x.ActDate);
            Map(x => x.AccountID);
            Map(x => x.Uses);
            Map(x => x.SchedDuration);
            Map(x => x.ActDuration);
            Map(x => x.OverTime);
            Map(x => x.Days);
            Map(x => x.Months);
            Map(x => x.IsStarted);
            Map(x => x.ChargeMultiplier);
            Map(x => x.ReservationID);
            Map(x => x.ChargeDuration);
            Map(x => x.TransferredDuration);
            Map(x => x.MaxReservedDuration);
            Map(x => x.ChargeBeginDateTime);
            Map(x => x.ChargeEndDateTime);
            Map(x => x.IsActive);
            Map(x => x.IsCancelledBeforeAllowedTime);
        }
    }
}
