using LNF.Models;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using System;

namespace LNF.CommonTools
{
    //TODO: add a function to end all reservations that ran past the end of the month
    //to this on the fourth business day
    //also, check that res.isactive=0 has null for act times, etc

    public class UpdateResult : ProcessResult
    {
        public DateTime Now { get; set; }
        public DateTime Period { get; set; }
        public bool IsFirstBusinessDay { get; set; }
        public UpdateTablesResult UpdateTablesResult { get; set; }
        public FinalizeResult FinalizeResult { get; set; }
        public override string ProcessName => "Update";

        protected override void WriteLog()
        {
            AppendLog($"Now: {Now:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"IsFirstBusinessDay: {IsFirstBusinessDay}");
            AppendResult(UpdateTablesResult);
            AppendResult(FinalizeResult);
        }
    }

    public class FinalizeResult : ProcessResult
    {
        public DateTime Period { get; set; }
        public WriteToolDataProcessResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataProcessResult WriteRoomDataProcessResult { get; set; }
        public WriteStoreDataProcessResult WriteStoreDataProcessResult { get; set; }
        public override string ProcessName => "Finalize";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendResult(WriteToolDataProcessResult);
            AppendResult(WriteRoomDataProcessResult);
            AppendResult(WriteStoreDataProcessResult);
        }
    }

    public static class DataTableManager
    {
        public static UpdateResult Update(BillingCategory types)
        {
            DateTime now = DateTime.Now;
            DateTime period = now.FirstOfMonth();

            WriteData wd = new WriteData();

            var result = new UpdateResult
            {
                Now = now,
                Period = period,
                IsFirstBusinessDay = Utility.IsFirstBusinessDay(now),
                //First, update tables
                UpdateTablesResult = wd.UpdateTables(types, UpdateDataType.DataClean | UpdateDataType.Data, period, 0)
            };

            //the daily update DOES produce correct data
            //however, if a change was made since the update, it needs to be caught 
            if (result.IsFirstBusinessDay)
                result.FinalizeResult = Finalize(period.AddMonths(-1));

            return result;
        }

        public static FinalizeResult Finalize(DateTime period)
        {
            return new FinalizeResult
            {
                Period = period,
                WriteToolDataProcessResult = new WriteToolDataProcess(period).Start(),
                WriteRoomDataProcessResult = new WriteRoomDataProcess(period).Start(),
                WriteStoreDataProcessResult = new WriteStoreDataProcess(period).Start()
            };
        }
    }
}
