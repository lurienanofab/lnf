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
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .MapSchema()
                    .ApplyParameters(new { Action = "Check", sDate = now.AddDays(-14) })
                    .FillDataTable("Holiday_Select");
        }
        public static bool IsHoliday(DateTime now)
        {
            using (var dba = DA.Current.GetAdapter())
            using (var reader = dba.ApplyParameters(new { Action = "IsHoliday", sDate = now }).ExecuteReader("Holiday_Select"))
                return reader.Read();
        }
    }
}
