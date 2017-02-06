using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Scheduler;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationOnTheFlyMap : ClassMap<ReservationOnTheFly>
    {
        public ReservationOnTheFlyMap()
        {
            Schema("sselScheduler.dbo");
            Id(x => x.ReservationOnTheFlyID);
            References(x => x.Reservation);
            Map(x => x.CardNumber);
            Map(x => x.OnTheFlyName);
			Map(x => x.IPAddress);
			Map(x => x.CardReaderName);
        }
    }
}
