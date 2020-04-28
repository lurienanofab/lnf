using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class LogPropertyValueMap : ClassMap<LogPropertyValue>
    {
        internal LogPropertyValueMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.LogPropertyValueID);
            References(x => x.LogProperty);
            Map(x => x.Text);
            Map(x => x.Value);
        }
    }
}