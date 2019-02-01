namespace LNF.Models.Billing.Process
{
    public class BillingProcessDataResult : ProcessResult
    {
        public override string ProcessName => "BillingProcessDataResult";

        public WriteToolDataProcessResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataProcessResult WriteRoomDataProcessResult { get; set; }
        public WriteStoreDataProcessResult WriteStoreDataProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(WriteToolDataProcessResult);
            AppendResult(WriteRoomDataProcessResult);
            AppendResult(WriteStoreDataProcessResult);
        }
    }
}
