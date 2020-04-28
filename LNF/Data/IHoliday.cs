using System;

namespace LNF.Data
{
    public interface IHoliday
    {
        int HolidayID { get; set; }
        string Description { get; set; }
        DateTime HolidayDate { get; set; }
    }
}