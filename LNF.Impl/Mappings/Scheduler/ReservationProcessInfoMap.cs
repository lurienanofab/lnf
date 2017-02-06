using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationProcessInfoMap : ClassMap<ReservationProcessInfo>
    {
        public ReservationProcessInfoMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ReservationProcessInfoID);
            References(x => x.Reservation);
            References(x => x.ProcessInfoLine);
            Map(x => x.Value).Not.Nullable();
            Map(x => x.Special).Not.Nullable();
            Map(x => x.RunNumber).Not.Nullable();
            //Map(x => x.ForgivenessPercentage).Not.Nullable();
            Map(x => x.Active);
        }
    }
}
