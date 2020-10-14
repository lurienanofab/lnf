using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class ResourceActivityAuth : IDataItem, IResourceActivityAuth
    {
        public virtual int ResourceActivityAuthID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual ClientAuthLevel UserAuth { get; set; }
        public virtual ClientAuthLevel InviteeAuth { get; set; }
        public virtual ClientAuthLevel StartEndAuth { get; set; }
        public virtual ClientAuthLevel NoReservFenceAuth { get; set; }
        public virtual ClientAuthLevel NoMaxSchedAuth { get; set; }
    }
}