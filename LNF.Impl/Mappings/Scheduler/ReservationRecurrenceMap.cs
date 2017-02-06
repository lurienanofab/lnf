using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationRecurrenceMap : ClassMap<ReservationRecurrence>
    {
        public ReservationRecurrenceMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.RecurrenceID);
            References(x => x.Resource);
            References(x => x.Client);
            Map(x => x.EndTime);
            Map(x => x.IsActive);
            References(x => x.Account);
            References(x => x.Activity);
            Map(x => x.CreatedOn);
            Map(x => x.AutoEnd);
            Map(x => x.KeepAlive);
            References(x => x.Pattern);
            Map(x => x.PatternParam1);
            Map(x => x.PatternParam2);
            Map(x => x.BeginDate);
            Map(x => x.EndDate);
            Map(x => x.BeginTime);
            Map(x => x.Duration);
            Map(x => x.Notes);
        }
    }
}
