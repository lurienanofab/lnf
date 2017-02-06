using LNF.Logging;
using LNF.Models.Billing;
using System;

namespace LNF.CommonTools
{
    //TODO: add a function to end all reservations that ran past the end of the month
    //to this on the fourth business day
    //also, check that res.isactive=0 has null for act times, etc

    public static class DataTableManager
    {
        public static void Update(string[] types, bool isDailyImport = true)
        {
            DateTime now = DateTime.Now;

            //this function runs daily
            DateTime eDate = now.Date; //00:00 of current day

            WriteData wd = new WriteData();

            //First, update tables
            wd.UpdateTables(types, 0, 0, eDate, UpdateDataType.DataClean | UpdateDataType.Data, true, isDailyImport);

            //the daily update DOES produce correct data
            //however, if a change was made since the update, it needs to be caught 
            if (Utility.IsFirstBusinessDay(now))
            {
                DateTime sDate = new DateTime(eDate.AddMonths(-1).Year, eDate.AddMonths(-1).Month, 1);
                eDate = sDate.AddMonths(1);
                Finalize(sDate, eDate);
            }
        }

        public static void Finalize(DateTime sd, DateTime ed)
        {
            using (LogTaskTimer.Start("DataTableManager.Finalize", "Finalizing data tables (room, tool, store). From {0:yyyy-MM-dd} to {1:yyyy-MM-dd}", () => new object[] { sd, ed }))
            { 
                WriteToolDataManager.Create(sd, ed).WriteToolData();
                WriteRoomDataManager.Create(sd, ed).WriteRoomData();
                WriteStoreDataManager.Create(sd, ed).WriteStoreData();
            }
        }
    }
}
