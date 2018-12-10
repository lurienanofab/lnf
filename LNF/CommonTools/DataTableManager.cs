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
        public static void Update(string[] types)
        {
            DateTime now = DateTime.Now;

            //this function runs daily
            DateTime period = now.FirstOfMonth(); //00:00 of current day

            WriteData wd = new WriteData();

            //First, update tables
            wd.UpdateTables(types, UpdateDataType.DataClean | UpdateDataType.Data, period, 0);

            //the daily update DOES produce correct data
            //however, if a change was made since the update, it needs to be caught 
            if (Utility.IsFirstBusinessDay(now))
            {
                Finalize(period.AddMonths(-1));
            }
        }

        public static void Finalize(DateTime period)
        {
            new WriteToolDataProcess(period).Start();
            new WriteRoomDataProcess(period).Start();
            new WriteStoreDataProcess(period).Start();
        }
    }
}
