using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class LogPropertyMap : ClassMap<LogProperty>
    {
        internal LogPropertyMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.LogPropertyID);
            Map(x => x.PropertyName);
        }
    }
}