using LNF.Data;
using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationInviteeInfo : IReservationInvitee, IDataItem
    {
        public virtual int ReservationID { get; set; }
        public virtual int InviteeID { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);
        public virtual ClientPrivilege Privs { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Removed { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ReservationInviteeInfo item)) return false;
            return item.ReservationID == ReservationID && item.InviteeID == InviteeID;
        }

        public override int GetHashCode()
        {
            return ReservationID.GetHashCode() * 17 + InviteeID.GetHashCode();
        }
    }
}
