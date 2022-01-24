using System;

namespace LNF.Billing.Process
{
    public class FinalizeResult : ProcessResult
    {
        protected FinalizeResult() { }

        public FinalizeResult(DateTime startedAt) : base(startedAt, null) { }

        public virtual DateTime Period { get; set; }
        public virtual WriteToolDataResult WriteToolDataProcessResult { get; set; }
        public virtual WriteRoomDataResult WriteRoomDataProcessResult { get; set; }
        public virtual WriteStoreDataResult WriteStoreDataProcessResult { get; set; }
        public virtual PopulateToolBillingResult PopulateToolBillingProcessResult { get; set; }
        public virtual PopulateRoomBillingResult PopulateRoomBillingProcessResult { get; set; }
        public virtual PopulateStoreBillingResult PopulateStoreBillingProcessResult { get; set; }
        public virtual PopulateSubsidyBillingResult PopulateSubsidyBillingProcessResult { get; set; }
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