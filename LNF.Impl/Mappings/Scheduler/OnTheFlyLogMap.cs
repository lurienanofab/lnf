using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class OnTheFlyLogMap : ClassMap<OnTheFlyLog>
    {
        internal OnTheFlyLogMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.OnTheFlyLogID);
            Map(x => x.LogGuid);
            Map(x => x.LogTimeStamp);
            Map(x => x.ActionName);
            Map(x => x.ActionData);
            Map(x => x.ResourceID);
            Map(x => x.IPAddress);
        }
    }
}
