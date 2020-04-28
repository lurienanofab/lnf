using LNF.Data;
using System;

namespace LNF.Scheduler
{
    public interface IReservationInvitee
    {
        bool Active { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
        DateTime BeginDateTime { get; set; }
        string DisplayName { get; }
        DateTime EndDateTime { get; set; }
        string FName { get; set; }
        int InviteeID { get; set; }
        bool IsActive { get; set; }
        bool IsStarted { get; set; }
        string LName { get; set; }
        ClientPrivilege Privs { get; set; }
        bool Removed { get; set; }
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
    }
}