using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;

namespace LNF.Scheduler
{
    public class ReservationInviteeItem
    {
        public int ReservationID { get; set; }
        public int InviteeID { get; set; }
        public string DisplayName { get; set; }

        public static ReservationInviteeItem Create(ReservationInvitee source)
        {
            return new ReservationInviteeItem()
            {
                ReservationID = source.Reservation.ReservationID,
                InviteeID = source.Invitee.ClientID,
                DisplayName = Client.GetDisplayName(source.Invitee.LName, source.Invitee.FName)
            };
        }

        public ReservationInvitee Find()
        {
            ReservationInvitee key = new ReservationInvitee()
            {
                Reservation = DA.Current.Single<Reservation>(ReservationID),
                Invitee = DA.Current.Single<Client>(InviteeID)
            };

            if (key.Reservation == null || key.Invitee == null)
                return null;

            return Find(key);
        }

        public static ReservationInvitee Find(ReservationInvitee key)
        {
            return DA.Current.Single<ReservationInvitee>(key);
        }

        public bool Exists()
        {
            return Find() != null;
        }

        public static bool Exists(ReservationInvitee key)
        {
            return Find(key) != null;
        }

        /// <summary>
        /// Adds a ReservationInvitee record if it does not already exist.
        /// </summary>
        public void Insert()
        {
            ReservationInvitee ri = new ReservationInvitee()
            {
                Reservation = DA.Current.Single<Reservation>(ReservationID),
                Invitee = DA.Current.Single<Client>(InviteeID)
            };

            if (!Exists(ri))
                DA.Current.Insert(ri);
        }

        /// <summary>
        /// Deletes an existing ReservationInvitee record.
        /// </summary>
        public void Delete()
        {
            var ri = Find();

            if (ri != null)
                DA.Current.Delete(ri);
        }
    }
}
