using LNF.DataAccess;
using LNF.Reporting;

namespace LNF.Impl.Repository.Reporting
{
    public class AfterHours : IAfterHours, IDataItem
    {
        public virtual int AfterHoursID { get; set; }
        public virtual string AfterHoursName { get; set; }
        public virtual int DayOfWeekIndex { get; set; }
        public virtual int HourIndex { get; set; }
        public virtual bool IsAfterHours { get; set; }
    }
}
