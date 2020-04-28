using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationLogMap : ClassMap<ReservationLog>
    {
        internal ReservationLogMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ReservationLogID);
            References(x => x.ResourceLogProperty);
            References(x => x.Reservation);
            Map(x => x.Value);
        }
    }
}