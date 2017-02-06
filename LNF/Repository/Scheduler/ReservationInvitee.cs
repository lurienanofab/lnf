using LNF.Repository.Data;

namespace LNF.Repository.Scheduler
{
    public class ReservationInvitee : IDataItem
    {
        //private int _hashCode = 0;
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
