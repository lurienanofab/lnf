namespace LNF.Repository.Reporting
{
    public class AfterHours : IDataItem
    {
        public virtual int AfterHoursID { get; set; }
        public virtual string AfterHoursName { get; set; }
        public virtual int DayOfWeekIndex { get; set; }
        public virtual int HourIndex { get; set; }
        public virtual bool IsAfterHours { get; set; }
    }
}
