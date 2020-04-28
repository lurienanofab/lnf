using System;

namespace LNF.Scheduler
{
    public interface IAutoEndLog
    {
        int AutoEndLogID { get; set; }
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        int ClientID { get; set; }
        string DisplayName { get; set; }
        DateTime Timestamp { get; set; }
        string Action { get; set; }
    }
}
