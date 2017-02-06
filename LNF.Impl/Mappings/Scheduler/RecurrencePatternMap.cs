using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class RecurrencePatternMap : ClassMap<RecurrencePattern>
    {
        public RecurrencePatternMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.PatternID);
            Map(x => x.PatternName);
            HasMany(x => x.ReservationRecurrences).KeyColumn("PatternID");
        }
    }
}
