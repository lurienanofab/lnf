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
            Map(x => x.ReservationID);
            Map(x => x.ProcessInfoLineID);
            Map(x => x.Value).Not.Nullable();
            Map(x => x.Special).Not.Nullable();
            Map(x => x.RunNumber).Nullable();
            Map(x => x.ChargeMultiplier).Nullable();
            Map(x => x.Active).Nullable();
        }
    }
}
