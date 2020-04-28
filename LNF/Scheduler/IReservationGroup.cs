using System;

namespace LNF.Scheduler
{
    public interface IReservationGroup
    {
        int GroupID { get; set; }
        int ClientID { get; set; }
        int AccountID { get; set; }
        int ActivityID { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        DateTime CreatedOn { get; set; }
        bool IsActive { get; set; }
    }
}
