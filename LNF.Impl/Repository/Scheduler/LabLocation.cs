using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class LabLocation : ILabLocation, IDataItem
    {
        public virtual int LabLocationID { get; set; }
        public virtual int LabID { get; set; }
        public virtual string LocationName { get; set; }
    }
}
