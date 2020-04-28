using System;

namespace LNF.Billing.Process
{
    public class FinalizeResult : ProcessResult
    {
        public DateTime Period { get; set; }
        public WriteToolDataResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataResult WriteRoomDataProcessResult { get; set; }
        public WriteStoreDataResult WriteStoreDataProcessResult { get; set; }
        public PopulateToolBillingResult PopulateToolBillingProcessResult { get; set; }
        public PopulateRoomBillingResult PopulateRoomBillingProcessResult { get; set; }
        public PopulateStoreBillingResult PopulateStoreBillingProcessResult { get; set; }
        public PopulateSubsidyBillingResult PopulateSubsidyBillingProcessResult { get; set; }
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
