using System;

namespace LNF.Billing.Process
{
    public class DataCleanResult : ProcessResult
    {
        protected DataCleanResult() { }

        public DataCleanResult(DateTime startedAt) : base(startedAt, null) { }

        public override string ProcessName => "BillingProcessDataCleanResult";

        public virtual WriteToolDataCleanResult WriteToolDataCleanProcessResult { get; set; }
        public virtual WriteRoomDataCleanResult WriteRoomDataCleanProcessResult { get; set; }
        public virtual WriteStoreDataCleanResult WriteStoreDataCleanProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(WriteToolDataCleanProcessResult);
            AppendResult(WriteRoomDataCleanProcessResult);
            AppendResult(WriteStoreDataCleanProcessResult);
        }
    }
}