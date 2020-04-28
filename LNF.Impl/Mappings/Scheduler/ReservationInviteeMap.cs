using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Mappings.Scheduler
{
    internal class ReservationInviteeMap : ClassMap<ReservationInvitee>
    {
        internal ReservationInviteeMap()
        {
            Schema("sselScheduler.dbo");
            CompositeId()
                .KeyReference(x => x.Reservation, "ReservationID")
                .KeyReference(x => x.Invitee, "InviteeID");
        }
    }
}
