using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ActivityAuthTypeMap : ClassMap<ActivityAuthType>
    {
        internal ActivityAuthTypeMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ActivityAuthTypeID);
            Map(x => x.AuthTypeName);
            Map(x => x.AuthTypeDescription);
        }
    }
}
