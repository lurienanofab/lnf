using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationProcessInfoInfoMap : ClassMap<ReservationProcessInfoInfo>
    {
        internal ReservationProcessInfoInfoMap()
        {
            Schema("sselScheduler.dbo");
            Table("v_ReservationProcessInfoInfo");
            ReadOnly();
            Id(x => x.ReservationProcessInfoID);
            Map(x => x.ReservationID);
            Map(x => x.Value);
            Map(x => x.Special);
            Map(x => x.RunNumber);
            Map(x => x.ChargeMultiplier);
            Map(x => x.Active);
            Map(x => x.ProcessInfoLineID);
            Map(x => x.Param);
            Map(x => x.ProcessInfoID);
            Map(x => x.ProcessInfoName);
        }
    }
}
