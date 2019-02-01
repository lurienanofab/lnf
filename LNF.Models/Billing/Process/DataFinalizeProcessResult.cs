using System;

namespace LNF.Models.Billing.Process
{
    public class DataFinalizeProcessResult : ProcessResult
    {
        public DateTime Period { get; set; }
        public WriteToolDataProcessResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataProcessResult WriteRoomDataProcessResult { get; set; }
        public WriteStoreDataProcessResult WriteStoreDataProcessResult { get; set; }
        public override string ProcessName => "DataFinalize";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendResult(WriteToolDataProcessResult);
            AppendResult(WriteRoomDataProcessResult);
            AppendResult(WriteStoreDataProcessResult);
        }
    }
}
