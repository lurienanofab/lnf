using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class AutoEndLogMap : ClassMap<AutoEndLog>
    {
        internal AutoEndLogMap()
        {
            Schema("sselScheduler.dbo");
            Table("AutoEndLog");
            Id(x => x.AutoEndLogID);
            Map(x => x.ReservationID);
            Map(x => x.ResourceID);
            Map(x => x.ResourceName);
            Map(x => x.ClientID);
            Map(x => x.DisplayName);
            Map(x => x.Timestamp);
            Map(x => x.Action);
        }
    }
}
