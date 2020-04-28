using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class AvailableInviteeMap : ClassMap<AvailableInvitee>
    {
        internal AvailableInviteeMap()
        {
            Schema("sselScheduler.dbo");
            ReadOnly();
            Id(x => x.ClientID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.DisplayName);
        }
    }
}
