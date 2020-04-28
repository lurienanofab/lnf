using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IHolidayRepository
    {
        IEnumerable<IHoliday> GetHolidays(DateTime sd, DateTime ed);
        IEnumerable<IHoliday> GetHolidays(DateTime now);
        bool IsHoliday(DateTime now);
    }
}