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
            Map(x => x.ReservationID).Not.Nullable();
            Map(x => x.ProcessInfoLineID).Not.Nullable();
            Map(x => x.Value).Not.Nullable();
            Map(x => x.Special).Not.Nullable();
            Map(x => x.RunNumber).Not.Nullable();
            Map(x => x.ChargeMultiplier).Not.Nullable();
            Map(x => x.Active).Not.Nullable();
        }
    }
}
