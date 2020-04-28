using LNF.DataAccess;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationInvitee : IDataItem
    {
        public virtual Reservation Reservation { get; set; }
        public virtual Client Invitee { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!typeof(ReservationInvitee).IsAssignableFrom(obj.GetType())) return false;

            var item = (ReservationInvitee)obj;

            if (item.Reservation.ReservationID == Reservation.ReservationID && item.Invitee.ClientID == Invitee.ClientID)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return new { Reservation.ReservationID, Invitee.ClientID }.GetHashCode();
        }
    }
}
