using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ToolDataRawMap : ClassMap<ToolDataRaw> 
    {
        internal ToolDataRawMap()
        {
            Schema("Billing.dbo");
            Table("v_ToolDataRaw");
            ReadOnly();
            Id(x => x.ReservationID);
            Map(x => x.ResourceID);
            Map(x => x.ClientID);
            Map(x => x.RoomID);
            Map(x => x.AccountID);
            Map(x => x.ActivityID);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.SchedDuration);
            Map(x => x.ActDuration);
            Map(x => x.OverTime);
            Map(x => x.IsActive);
            Map(x => x.IsStarted);
            Map(x => x.ChargeMultiplier);
            Map(x => x.MaxReservedDuration);
            Map(x => x.CancelledDateTime);
            Map(x => x.OriginalBeginDateTime);
            Map(x => x.OriginalEndDateTime);
            Map(x => x.OriginalModifiedOn);
            Map(x => x.CreatedOn);
        }
    }
}
