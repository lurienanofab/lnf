using LNF.Data;
using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public abstract class ReservationInviteeBase : IReservationInviteeItem
    {
        public virtual int InviteeID { get; set; }
        public virtual int ReservationID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool InviteeActive { get; set; }
        public virtual string InviteeLName { get; set; }
        public virtual string InviteeFName { get; set; }
        public virtual string InviteeDisplayName => Clients.GetDisplayName(InviteeLName, InviteeFName);
        public virtual ClientPrivilege InviteePrivs { get; set; }
    }

    public class ReservationInviteeItem : ReservationInviteeBase, IDataItem
    {
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ReservationInviteeItem item)) return false;
            return item.ReservationID == ReservationID && item.InviteeID == InviteeID;
        }

        public override int GetHashCode()
        {
            return ReservationID.GetHashCode() * 17 + InviteeID.GetHashCode();
        }
    }
}
