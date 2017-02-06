namespace LNF.Models.Scheduler
{
    /// <summary>
    /// An invitee to a reservation
    /// </summary>
    public class ReservationInviteeModel
    {
        /// <summary>
        /// The id for the Reservation
        /// </summary>
        public int ReservationID { get; set; }

        /// <summary>
        /// The id for the Client that was invited
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// The last name of the Client who was invited
        /// </summary>
        public string LName { get; set; }

        /// <summary>
        /// The first name of the Client who was invited
        /// </summary>
        public string FName { get; set; }
    }
}
