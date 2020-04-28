using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationOnTheFlyMap : ClassMap<ReservationOnTheFly>
    {
        internal ReservationOnTheFlyMap()
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
