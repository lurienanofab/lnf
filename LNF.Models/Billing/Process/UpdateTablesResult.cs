using System;

namespace LNF.Models.Billing.Process
{
    public class UpdateTablesResult : ProcessResult
    {
        public BillingCategory BillingTypes { get; set; }
        public UpdateDataType UpdateTypes { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public WriteToolDataCleanProcessResult WriteToolDataCleanProcessResult { get; set; }
        public WriteRoomDataCleanProcessResult WriteRoomDataCleanProcessResult { get; set; }
        public WriteStoreDataCleanProcessResult WriteStoreDataCleanProcessResult { get; set; }
        public WriteToolDataProcessResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataProcessResult WriteRoomDataProcessResult { get; set; }
        public WriteStoreDataProcessResult WriteStoreDataProcessResult { get; set; }
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
