using LNF.Models.Data;
using System;

namespace LNF.Models.Scheduler
{
    /// <summary>
    /// An invitee to a reservation
    /// </summary>
    public class ReservationInviteeItem : IReservationInvitee
    {
        public int ReservationID { get; set; }
        public int InviteeID { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public bool IsStarted { get; set; }
        public bool IsActive { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public bool Active { get; set; }

        /// <summary>
        /// Indicates whether or not this invitee was removed from a reservation or not.
        /// </summary>
        public bool Removed { get; set; }

        public string DisplayName => ClientItem.GetDisplayName(LName, FName);
    }
}
