namespace LNF.Billing.Process
{
    public class Step1Result : ProcessResult
    {
        public override string ProcessName => "BillingProcessStep1Result";

        public PopulateToolBillingResult PopulateToolBillingProcessResult { get; set; }
        public PopulateRoomBillingResult PopulateRoomBillingProcessResult { get; set; }
        public PopulateStoreBillingResult PopulateStoreBillingProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(PopulateToolBillingProcessResult);
            AppendResult(PopulateRoomBillingProcessResult);
            AppendResult(PopulateStoreBillingProcessResult);
        }
    }
}
