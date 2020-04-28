using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class AuthLevelMap : ClassMap<AuthLevel>
    {
        internal AuthLevelMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.AuthLevelID).GeneratedBy.Assigned();
            Map(x => x.AuthLevelName);
            Map(x => x.Authorizable);
        }
    }
}
