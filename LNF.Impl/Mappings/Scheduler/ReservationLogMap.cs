using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationLogMap : ClassMap<ReservationLog>
    {
        public ReservationLogMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ReservationLogID);
            References(x => x.ResourceLogProperty);
            References(x => x.Reservation);
            Map(x => x.Value);
        }
    }
}