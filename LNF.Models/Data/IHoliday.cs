using System;

namespace LNF.Models.Data
{
    public interface IHoliday
    {
        string Description { get; set; }
        DateTime HolidayDate { get; set; }
        int HolidayID { get; set; }
    }
}