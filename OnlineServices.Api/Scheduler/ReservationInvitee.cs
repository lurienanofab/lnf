using LNF.Data;
using LNF.Scheduler;
using System;

namespace OnlineServices.Api.Scheduler
{
    public class ReservationInvitee : IReservationInvitee
    {
        public bool Active { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime BeginDateTime { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public DateTime EndDateTime { get; set; }
        public string FName { get; set; }
        public int InviteeID { get; set; }
        public bool IsActive { get; set; }
        public bool IsStarted { get; set; }
        public string LName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public bool Removed { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
    }
}
