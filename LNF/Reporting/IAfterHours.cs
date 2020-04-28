namespace LNF.Reporting
{
    public interface IAfterHours
    {
        int AfterHoursID { get; set; }
        string AfterHoursName { get; set; }
        int DayOfWeekIndex { get; set; }
        int HourIndex { get; set; }
        bool IsAfterHours { get; set; }
    }
}
