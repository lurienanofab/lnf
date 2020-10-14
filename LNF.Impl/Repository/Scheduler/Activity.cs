using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class Activity : IDataItem, IActivity
    {
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual int ListOrder { get; set; }
        public virtual bool Chargeable { get; set; }
        public virtual bool Editable { get; set; }
        public virtual ActivityAccountType AccountType { get; set; }
        public virtual ClientAuthLevel UserAuth { get; set; }
        public virtual ActivityInviteeType InviteeType { get; set; }
        public virtual ClientAuthLevel InviteeAuth { get; set; }
        public virtual ClientAuthLevel StartEndAuth { get; set; }
        public virtual ClientAuthLevel NoReservFenceAuth { get; set; }
        public virtual ClientAuthLevel NoMaxSchedAuth { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsFacilityDownTime { get; set; }
        public virtual bool IsRepair => !Editable;

        public override string ToString()
        {
            return $"{ActivityName} [{ActivityID}]";
        }
    }
}
