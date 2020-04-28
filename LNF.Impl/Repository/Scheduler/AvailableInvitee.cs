using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class AvailableInvitee : IAvailableInvitee, IDataItem
    {
        public virtual int ClientID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string DisplayName { get; set; }
    }
}
