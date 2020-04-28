using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ResourceClientMap : ClassMap<ResourceClient>
    {
        internal ResourceClientMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ResourceClientID);
            Map(x => x.ResourceID);
            Map(x => x.ClientID);
            Map(x => x.AuthLevel);
            Map(x => x.Expiration);
            Map(x => x.EmailNotify);
            Map(x => x.PracticeResEmailNotify);
        }
    }
}
