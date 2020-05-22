using LNF.Data;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationInviteeInfo : ReservationInfoBase, IReservationInvitee
    {
        public virtual int InviteeID { get; set; }
        public virtual string InviteeLName { get; set; }
        public virtual string InviteeFName { get; set; }
        public virtual ClientPrivilege InviteePrivs { get; set; }
        public virtual bool InviteeActive { get; set; }
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
