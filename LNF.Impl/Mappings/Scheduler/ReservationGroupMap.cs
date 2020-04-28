using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationGroupMap : ClassMap<ReservationGroup>
    {
        internal ReservationGroupMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.GroupID);
            References(x => x.Client, "ClientID");
            References(x => x.Account, "AccountID");
            References(x => x.Activity, "ActivityID");
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.CreatedOn);
            Map(x => x.IsActive);
        }
    }
}
