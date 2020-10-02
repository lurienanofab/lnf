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
            var holidays = ServiceProvider.Current.Data.Holiday.GetHolidays(now);

            var dt = new DataTable();

            dt.Columns.Add("HolidayID", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("HolidayDate", typeof(DateTime));

            dt.PrimaryKey = new[] { dt.Columns["HolidayID"] };

            foreach(var h in holidays)
            {
                var ndr = dt.NewRow();
                ndr.SetField("HolidayID", h.HolidayID);
                ndr.SetField("Description", h.Description);
                ndr.SetField("HolidayDate", h.HolidayDate);
                dt.Rows.Add(ndr);
            }

            return dt;
        }
    }
}
