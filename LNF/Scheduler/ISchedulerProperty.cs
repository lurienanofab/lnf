namespace LNF.Scheduler
{
    public interface ISchedulerProperty
    {
        int PropertyID { get; set; }
        string PropertyName { get; set; }
        string PropertyValue { get; set; }
    }
}