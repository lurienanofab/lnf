using System;

namespace LNF.Scheduler
{
    public class RecentReservation
    {
        public int ReservationID { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
    }
}
