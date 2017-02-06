using LNF.CommonTools;
using LNF.Models.Billing;
using System;

namespace LNF.Billing
{
    public static class DataTableManager
    {
        public static void DailyUpdate(BillingCategory billingCategory)
        {
            DateTime now = DateTime.Now;

            //this function runs daily
            DateTime eDate = new DateTime(now.Year, now.Month, now.Day); //00:00 of current day

            DataTableWriter writer = new DataTableWriter(billingCategory, 0, 0, eDate, UpdateDataType.DataClean | UpdateDataType.Data, true, true);

            //First, update tables
            writer.Update();

            //the daily update DOES produce correct data
            //however, if a change was made since the update, it needs to be caught 
            if (Utility.IsFirstBusinessDay(now))
            {
                DateTime sDate = new DateTime(eDate.AddMonths(-1).Year, eDate.AddMonths(-1).Month, 1);
                eDate = sDate.AddMonths(1);
                //Finalize(sDate, eDate);
            }
        }
    }
}
