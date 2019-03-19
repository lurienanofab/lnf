using System;

namespace LNF.Models.Scheduler
{
    public interface IReservation
    {
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        int ClientID { get; set; }
        int AccountID { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
    }
}
