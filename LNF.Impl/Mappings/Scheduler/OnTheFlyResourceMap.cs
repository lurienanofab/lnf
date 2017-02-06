using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class OnTheFlyResourceMap : ClassMap<OnTheFlyResource>
    {
        public OnTheFlyResourceMap()
        {
            Schema("sselScheduler.dbo");
            Id(x=>x.OnTheFlyResourceID);
            Map(x => x.CardReaderName);
			Map(x => x.ButtonIndex);
            References(x => x.Resource);
			Map(x => x.ResourceType);
            Map(x => x.ResourceStateDuration);
			Map(x => x.CardSwipeAction);
        }
    }
}
