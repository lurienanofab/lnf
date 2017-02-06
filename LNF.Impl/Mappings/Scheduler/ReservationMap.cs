using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationMap : ClassMap<Reservation>
    {
        public ReservationMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ReservationID);
            References(x => x.Resource);
            References(x => x.Client);
            References(x => x.Account);
            References(x => x.Activity);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ActualBeginDateTime);
            Map(x => x.ActualEndDateTime);
            Map(x => x.ClientIDBegin);
            Map(x => x.ClientIDEnd);
            Map(x => x.CreatedOn);
            Map(x => x.LastModifiedOn);
            Map(x => x.Duration);
            Map(x => x.Notes);
            Map(x => x.ChargeMultiplier);
            Map(x => x.ApplyLateChargePenalty);
            Map(x => x.AutoEnd);
            Map(x => x.HasProcessInfo);
            Map(x => x.HasInvitees);
            Map(x => x.IsActive);
            Map(x => x.IsStarted);
            Map(x => x.IsUnloaded);
            Map(x => x.RecurrenceID);
            Map(x => x.GroupID);
            Map(x => x.MaxReservedDuration);
            Map(x => x.CancelledDateTime);
            Map(x => x.KeepAlive);
            Map(x => x.OriginalBeginDateTime);
            Map(x => x.OriginalEndDateTime);
            Map(x => x.OriginalModifiedOn);
        }
    }
}
