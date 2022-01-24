using LNF.CommonTools;
using System;

namespace LNF.Billing.Process
{
    public class UpdateResult : ProcessResult
    {
        protected UpdateResult() { }

        public UpdateResult(DateTime startedAt) : base(startedAt, null) { }

        public virtual BillingCategory BillingTypes { get; set; }
        public virtual UpdateDataType UpdateTypes { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual WriteToolDataCleanResult WriteToolDataCleanProcessResult { get; set; }
        public virtual WriteRoomDataCleanResult WriteRoomDataCleanProcessResult { get; set; }
        public virtual WriteStoreDataCleanResult WriteStoreDataCleanProcessResult { get; set; }
        public virtual WriteToolDataResult WriteToolDataProcessResult { get; set; }
        public virtual WriteRoomDataResult WriteRoomDataProcessResult { get; set; }
        public virtual WriteStoreDataResult WriteStoreDataProcessResult { get; set; }
        public virtual string Error { get; set; }
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