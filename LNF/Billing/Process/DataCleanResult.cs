namespace LNF.Billing.Process
{
    public class DataCleanResult : ProcessResult
    {
        public override string ProcessName => "BillingProcessDataCleanResult";

        public WriteToolDataCleanResult WriteToolDataCleanProcessResult { get; set; }
        public WriteRoomDataCleanResult WriteRoomDataCleanProcessResult { get; set; }
        public WriteStoreDataCleanResult WriteStoreDataCleanProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(WriteToolDataCleanProcessResult);
            AppendResult(WriteRoomDataCleanProcessResult);
            AppendResult(WriteStoreDataCleanProcessResult);
        }
    }
}
