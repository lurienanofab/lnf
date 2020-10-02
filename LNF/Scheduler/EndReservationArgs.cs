using System;

namespace LNF.Scheduler
{
    public class EndReservationArgs
    {
        public EndReservationArgs() { }

        public EndReservationArgs(int reservationId, DateTime actualEndDateTime, int endedByClientId)
        {
            ReservationID = reservationId;
            ActualEndDateTime = actualEndDateTime;
            EndedByClientID = endedByClientId;
        }

        public int ReservationID { get; set; }
        public DateTime ActualEndDateTime { get; set; }
        public int EndedByClientID { get; set; }
    }
}
