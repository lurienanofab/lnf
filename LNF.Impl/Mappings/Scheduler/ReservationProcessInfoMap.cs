using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationProcessInfoMap : ClassMap<ReservationProcessInfo>
    {
        internal ReservationProcessInfoMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ReservationProcessInfoID);
            References(x => x.Reservation);
            References(x => x.ProcessInfoLine);
            Map(x => x.Value).Not.Nullable();
            Map(x => x.Special).Not.Nullable();
            Map(x => x.RunNumber).Nullable();
            Map(x => x.ChargeMultiplier).Nullable();
            Map(x => x.Active).Nullable();
        }
    }
}
