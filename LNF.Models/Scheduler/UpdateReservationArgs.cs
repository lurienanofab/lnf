using System;

namespace LNF.Models.Scheduler
{
    public class UpdateReservationArgs
    {
        public int ReservationID { get; set; }
        public int AccountID { get; set; }
        public string Notes { get; set; }
        public double ChargeMultiplier { get; set; }
        public bool AutoEnd { get; set; }
        public bool KeepAlive { get; set; }
        public bool HasProcessInfo { get; set; }
        public bool HasInvitees { get; set; }
        public DateTime Now { get; set; }
        public int ModifiedByClientID { get; set; }
    }
}
