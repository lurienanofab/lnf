using System;

namespace LNF.Models.Data
{
    public class HolidayItem : IHoliday
    {
        public int HolidayID { get; set; }
        public string Description { get; set; }
        public DateTime HolidayDate { get; set; }
    }
}
