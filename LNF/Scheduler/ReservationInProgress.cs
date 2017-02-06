using System;

namespace LNF.Scheduler
{
    public class ReservationInProgress
    {
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public bool Editable { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Notes { get; set; }
    }
}
