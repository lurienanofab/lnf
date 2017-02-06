using LNF.Models.Scheduler;
using System.Linq;

namespace LNF.Repository.Scheduler
{
    public class Activity : IDataItem
    {
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual int ListOrder { get; set; }
        public virtual bool Chargeable { get; set; }
        public virtual bool Editable { get; set; }
        public virtual ActivityAccountType AccountType { get; set; }
        public virtual int UserAuth { get; set; }
        public virtual int InviteeType { get; set; }
        public virtual int InviteeAuth { get; set; }
        public virtual int StartEndAuth { get; set; }
        public virtual int NoReservFenceAuth { get; set; }
        public virtual int NoMaxSchedAuth { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsFacilityDownTime { get; set; }

        public virtual IQueryable<ReservationRecurrence> ReservationRecurrences()
        {
            return DA.Current.Query<ReservationRecurrence>().Where(x => x.Activity.ActivityID == ActivityID);
        }
    }
}
