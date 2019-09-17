using System;

namespace LNF.Models.Scheduler
{
    public class InsertReservationArgs
    {
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public int ResourceID { get; set; }
        public int ActivityID { get; set; }
        public int? RecurrenceID { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double ChargeMultiplier { get; set; }
        public bool AutoEnd { get; set; }
        public bool KeepAlive { get; set; }
        public bool HasInvitees { get; set; }
        public bool HasProcessInfo { get; set; }
        public string Notes { get; set; }
        public DateTime Now { get; set; }
        public int LinkedReservationID { get; set; }
        public int ModifiedByClientID { get; set; }
        public TimeSpan Duration => EndDateTime - BeginDateTime;
    }
}
