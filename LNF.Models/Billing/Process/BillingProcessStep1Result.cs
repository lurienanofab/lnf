namespace LNF.Models.Billing.Process
{
    public class BillingProcessStep1Result : ProcessResult
    {
        public override string ProcessName => "BillingProcessStep1Result";

        public PopulateToolBillingProcessResult PopulateToolBillingProcessResult { get; set; }
        public PopulateRoomBillingProcessResult PopulateRoomBillingProcessResult { get; set; }
        public PopulateStoreBillingProcessResult PopulateStoreBillingProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(PopulateToolBillingProcessResult);
            AppendResult(PopulateRoomBillingProcessResult);
            AppendResult(PopulateStoreBillingProcessResult);
        }
    }
}
