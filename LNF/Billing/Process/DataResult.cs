namespace LNF.Billing.Process
{
    public class DataResult : ProcessResult
    {
        public override string ProcessName => "BillingProcessDataResult";

        public WriteToolDataResult WriteToolDataProcessResult { get; set; }
        public WriteRoomDataResult WriteRoomDataProcessResult { get; set; }
        public WriteStoreDataResult WriteStoreDataProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(WriteToolDataProcessResult);
            AppendResult(WriteRoomDataProcessResult);
            AppendResult(WriteStoreDataProcessResult);
        }
    }
}
