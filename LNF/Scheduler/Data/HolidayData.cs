using LNF.Repository;
using System;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling Holiday data using the System.Data namespace.
    /// </summary>
    public static class HolidayData
    {
        /// <summary>
        /// Returns all holidays plus/minus two weeks from today
        /// </summary>
        public static DataTable SelectHolidays(DateTime now)
        {
            return DA.Command()
                .MapSchema()
                .Param(new { Action = "Check", sDate = now.AddDays(-14) })
                .FillDataTable("dbo.Holiday_Select");
        }
        public static bool IsHoliday(DateTime now)
        {
            using (var reader = DA.Command().Param(new { Action = "IsHoliday", sDate = now }).ExecuteReader("dbo.Holiday_Select"))
                return reader.Read();
        }
    }
}
