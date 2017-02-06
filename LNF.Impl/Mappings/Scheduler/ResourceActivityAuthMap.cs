using FluentNHibernate.Mapping;
using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceActivityAuthMap : ClassMap<ResourceActivityAuth>
    {
        internal ResourceActivityAuthMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceActivityAuthID);
            References(x => x.Resource, "ResourceID");
            References(x => x.Activity, "ActivityID");
            Map(x => x.UserAuth).CustomType<ClientAuthLevel>();
            Map(x => x.InviteeAuth).CustomType<ClientAuthLevel>();
            Map(x => x.StartEndAuth).CustomType<ClientAuthLevel>();
            Map(x => x.NoReservFenceAuth).CustomType<ClientAuthLevel>();
            Map(x => x.NoMaxSchedAuth).CustomType<ClientAuthLevel>();
        }
    }
}
