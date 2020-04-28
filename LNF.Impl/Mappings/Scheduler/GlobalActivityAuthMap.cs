using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class GlobalActivityAuthMap : ClassMap<GlobalActivityAuth>
    {
        internal GlobalActivityAuthMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.GlobalActivityAuthID);
            References(x => x.Activity);
            References(x => x.ActivityAuthType);
            Map(x => x.DefaultAuth);
            Map(x => x.LockedAuth);
        }
    }
}
