using FluentNHibernate.Mapping;
using LNF.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    public class ReservationInviteeMap : ClassMap<ReservationInvitee>
    {
        public ReservationInviteeMap()
        {
            Schema("sselScheduler.dbo");
            CompositeId()
                .KeyReference(x => x.Reservation, "ReservationID")
                .KeyReference(x => x.Invitee, "InviteeID");
        }
    }
}
