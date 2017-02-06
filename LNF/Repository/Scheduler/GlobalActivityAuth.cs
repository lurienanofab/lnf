using LNF.Models.Scheduler;

namespace LNF.Repository.Scheduler
{
    public class GlobalActivityAuth : IDataItem
    {
        public virtual int GlobalActivityAuthID { get; set; }
        public virtual ActivityAuthType ActivityAuthType { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual ClientAuthLevel DefaultAuth { get; set; }
        public virtual ClientAuthLevel LockedAuth { get; set; }
    }
}
