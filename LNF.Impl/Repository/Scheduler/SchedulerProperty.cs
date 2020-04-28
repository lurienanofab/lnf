using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class SchedulerProperty : ISchedulerProperty, IDataItem
    {
        public virtual int PropertyID { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string PropertyValue { get; set; }
    }
}
