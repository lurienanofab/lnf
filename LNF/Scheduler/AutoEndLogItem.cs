using System;

namespace LNF.Scheduler
{
    public class AutoEndLogItem : IAutoEndLog
    {
        public int AutoEndLogID { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
    }
}
