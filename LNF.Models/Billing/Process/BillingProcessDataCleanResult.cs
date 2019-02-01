namespace LNF.Models.Billing.Process
{
    public class BillingProcessDataCleanResult : ProcessResult
    {
        public override string ProcessName => "BillingProcessDataCleanResult";

        public WriteToolDataCleanProcessResult WriteToolDataCleanProcessResult { get; set; }
        public WriteRoomDataCleanProcessResult WriteRoomDataCleanProcessResult { get; set; }
        public WriteStoreDataCleanProcessResult WriteStoreDataCleanProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(WriteToolDataCleanProcessResult);
            AppendResult(WriteRoomDataCleanProcessResult);
            AppendResult(WriteStoreDataCleanProcessResult);
        }
    }
}
