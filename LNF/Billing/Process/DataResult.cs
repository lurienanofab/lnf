using System;

namespace LNF.Billing.Process
{
    public class DataResult : ProcessResult
    {
        protected DataResult() { }

        public DataResult(DateTime startedAt) : base(startedAt, null) { }

        public override string ProcessName => "BillingProcessDataResult";

        public virtual WriteToolDataResult WriteToolDataProcessResult { get; set; }
        public virtual WriteRoomDataResult WriteRoomDataProcessResult { get; set; }
        public virtual WriteStoreDataResult WriteStoreDataProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(WriteToolDataProcessResult);
            AppendResult(WriteRoomDataProcessResult);
            AppendResult(WriteStoreDataProcessResult);
        }
    }
}