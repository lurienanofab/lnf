using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF.CommonTools
{
    //TODO: add a function to end all reservations that ran past the end of the month
    //to this on the fourth business day
    //also, check that res.isactive=0 has null for act times, etc

    public static class DataTableManager
    {
        public static DataUpdateProcessResult Update(BillingCategory types)
        {
            DateTime now = DateTime.Now;
            DateTime period = now.FirstOfMonth();

            var holidays = Utility.GetHolidays(period, period.AddMonths(1));

            WriteData wd = new WriteData();

            var result = new DataUpdateProcessResult
            {
                Now = now,
                Period = period,
                IsFirstBusinessDay = Utility.IsFirstBusinessDay(now, holidays),
                //First, update tables
                UpdateTablesResult = wd.UpdateTables(types, UpdateDataType.DataClean | UpdateDataType.Data, period, 0)
            };

            //the daily update DOES produce correct data
            //however, if a change was made since the update, it needs to be caught 
            if (result.IsFirstBusinessDay)
                result.FinalizeResult = Finalize(period.AddMonths(-1));

            return result;
        }

        public static DataFinalizeProcessResult Finalize(DateTime period)
        {
            return new DataFinalizeProcessResult
            {
                Period = period,
                WriteToolDataProcessResult = new WriteToolDataProcess(period).Start(),
                WriteRoomDataProcessResult = new WriteRoomDataProcess(period).Start(),
                WriteStoreDataProcessResult = new WriteStoreDataProcess(period).Start()
            };
        }
    }
}
