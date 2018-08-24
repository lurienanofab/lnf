namespace LNF.Repository.Scheduler
{
    public class SchedulerProperty : IDataItem
    {
        public virtual int PropertyID { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string PropertyValue { get; set; }
    }
}
