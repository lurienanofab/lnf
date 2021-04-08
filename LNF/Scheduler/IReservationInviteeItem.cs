using LNF.Data;
using System;

namespace LNF.Scheduler
{
    public interface IReservationInviteeItem
    {
        int InviteeID { get; set; }
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        int ProcessTechID { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
        bool IsStarted { get; set; }
        bool IsActive { get; set; }
        bool InviteeActive { get; set; }
        string InviteeLName { get; set; }
        string InviteeFName { get; set; }
        string InviteeDisplayName { get; }
        ClientPrivilege InviteePrivs { get; set; }
    }
}