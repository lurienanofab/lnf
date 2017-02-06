using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationHistoryMap : ClassMap<ReservationHistory>
    {
        public ReservationHistoryMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ReservationHistoryID);
            References(x => x.Reservation);
            Map(x => x.LinkedReservationID);
            Map(x => x.UserAction);
            Map(x => x.ActionSource);
            Map(x => x.ModifiedByClientID);
            Map(x => x.ModifiedDateTime);
            References(x => x.Account);
            Map(x => x.BeginDateTime);
            Map(x => x.EndDateTime);
            Map(x => x.ChargeMultiplier);
        }
    }
}
