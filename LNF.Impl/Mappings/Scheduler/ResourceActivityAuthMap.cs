using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceActivityAuthMap : ClassMap<ResourceActivityAuth>
    {
        internal ResourceActivityAuthMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceActivityAuthID);
            Map(x => x.ResourceID);
            Map(x => x.ActivityID);
            Map(x => x.UserAuth).CustomType<ClientAuthLevel>();
            Map(x => x.InviteeAuth).CustomType<ClientAuthLevel>();
            Map(x => x.StartEndAuth).CustomType<ClientAuthLevel>();
            Map(x => x.NoReservFenceAuth).CustomType<ClientAuthLevel>();
            Map(x => x.NoMaxSchedAuth).CustomType<ClientAuthLevel>();
        }
    }
}
