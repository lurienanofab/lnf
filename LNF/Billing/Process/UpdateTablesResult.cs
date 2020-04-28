using LNF.CommonTools;
using System;

namespace LNF.Billing.Process
{
    public class UpdateResult : ProcessResult
    {
        public BillingCategory BillingTypes { get; set; }
        public UpdateDataType UpdateTypes { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public WriteToolDataCleanResult WriteToolDataCleanProcessResult { get; set; }
        public WriteRoomDataCleanResult WriteRoomDataCleanProcessResult { get; set; }
        public WriteStoreDataCleanResult WriteStoreDataCleanProcessResult { get; set; }
        public WriteToolDataResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataResult WriteRoomDataProcessResult { get; set; }
        public WriteStoreDataResult WriteStoreDataProcessResult { get; set; }
        public string Error { get; set; }
        public override string ProcessName => "UpdateTables";

        protected override void WriteLog()
        {
            AppendLog($"Types: {Utility.EnumToString(BillingTypes)}");
            AppendLog($"UpdateTypes: {Utility.EnumToString(UpdateTypes)}");
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            AppendLog($"Error: {(string.IsNullOrEmpty(Error) ? "none" : Error)}");
            AppendResult(WriteToolDataCleanProcessResult);
            AppendResult(WriteRoomDataCleanProcessResult);
            AppendResult(WriteStoreDataCleanProcessResult);
            AppendResult(WriteToolDataProcessResult);
            AppendResult(WriteRoomDataProcessResult);
            AppendResult(WriteStoreDataProcessResult);
        }
    }
}
