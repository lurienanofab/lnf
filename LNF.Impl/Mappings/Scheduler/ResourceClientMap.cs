using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ResourceClientMap : ClassMap<ResourceClient>
    {
        public ResourceClientMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceClientID);
            References(x => x.Resource, "ResourceID");
            Map(x => x.ClientID);
            Map(x => x.AuthLevel);
            Map(x => x.Expiration);
            Map(x => x.EmailNotify);
            Map(x => x.PracticeResEmailNotify);
        }
    }
}
