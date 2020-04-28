using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class OnTheFlyResourceMap : ClassMap<OnTheFlyResource>
    {
        internal OnTheFlyResourceMap()
        {
            Schema("sselScheduler.dbo");
            Id(x=>x.OnTheFlyResourceID);
            Map(x => x.CardReaderName);
			Map(x => x.ButtonIndex);
            Map(x => x.ResourceID);
			Map(x => x.ResourceType);
            Map(x => x.ResourceStateDuration);
			Map(x => x.CardSwipeAction);
        }
    }
}
