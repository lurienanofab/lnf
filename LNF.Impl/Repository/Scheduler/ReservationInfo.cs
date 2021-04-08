using System.Collections.Generic;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationInfo : ReservationInfoBase, IReservationItem { }

    public class ReservationWithInvitees : ReservationInfo, IReservationWithInvitees
    {
        public virtual IEnumerable<IReservationInviteeItem> Invitees { get; set; }
    }
}
