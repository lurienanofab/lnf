using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ToolDataCleanMap : ClassMap<ToolDataClean>
    {
        internal ToolDataCleanMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ToolDataID);
            Map(x => x.ClientID);
            Map(x => x.ResourceID);
            Map(x => x.RoomID);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.AccountID);
            Map(x => x.ActivityID);
            Map(x => x.SchedDuration);
            Map(x => x.ActDuration);
            Map(x => x.OverTime);
            Map(x => x.IsStarted);
            Map(x => x.ChargeMultiplier);
            Map(x => x.ReservationID);
            Map(x => x.MaxReservedDuration);
            Map(x => x.IsActive);
            Map(x => x.CancelledDateTime);
            Map(x => x.OriginalBeginDateTime);
            Map(x => x.OriginalEndDateTime);
            Map(x => x.OriginalModifiedOn);
            Map(x => x.CreatedOn);
            Map(x => x.LastModifiedOn);
        }
    }
}
