using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class RecurrencePatternMap : ClassMap<RecurrencePattern>
    {
        internal RecurrencePatternMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.PatternID);
            Map(x => x.PatternName);
            HasMany(x => x.ReservationRecurrences).KeyColumn("PatternID");
        }
    }
}
