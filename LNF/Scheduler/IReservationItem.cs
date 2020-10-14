using System;

namespace LNF.Scheduler
{
    public interface IReservationItem
    {
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
        bool IsStarted { get; set; }
        bool IsActive { get; set; }
        int ClientID { get; set; }
        int ProcessTechID { get; set; }
        int LabID { get; set; }
    }
}
