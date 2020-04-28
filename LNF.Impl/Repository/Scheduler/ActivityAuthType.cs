using LNF.DataAccess;

namespace LNF.Impl.Repository.Scheduler
{
    public class ActivityAuthType : IDataItem
    {
        public virtual int ActivityAuthTypeID { get; set; }
        public virtual string AuthTypeName { get; set; }
        public virtual string AuthTypeDescription { get; set; }
    }
}
