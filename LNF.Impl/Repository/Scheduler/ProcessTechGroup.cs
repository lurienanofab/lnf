using LNF.DataAccess;

namespace LNF.Impl.Repository.Scheduler
{
    public class ProcessTechGroup : IDataItem
    {
        public virtual int ProcessTechGroupID { get; set; }
        public virtual string GroupName { get; set; }
    }
}
