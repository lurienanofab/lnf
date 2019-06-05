using System;

namespace LNF.Models.Scheduler
{
    public interface IReservationRecurrence
    {
        int AccountID { get; set; }
        string AccountName { get; set; }
        int ActivityID { get; set; }
        string ActivityName { get; set; }
        bool AutoEnd { get; set; }
        DateTime BeginDate { get; set; }
        DateTime BeginTime { get; set; }
        int ClientID { get; set; }
        DateTime CreatedOn { get; set; }
        double Duration { get; set; }
        DateTime? EndDate { get; set; }
        DateTime EndTime { get; set; }
        string FName { get; set; }
        bool IsActive { get; set; }
        bool KeepAlive { get; set; }
        string LName { get; set; }
        string Notes { get; set; }
        int PatternID { get; set; }
        string PatternName { get; set; }
        int PatternParam1 { get; set; }
        int? PatternParam2 { get; set; }
        int RecurrenceID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
    }
}