using System;

namespace LNF.Billing.Process
{
    public class Step1Result : ProcessResult
    {
        protected Step1Result() { }

        public Step1Result(DateTime startedAt) : base(startedAt, null) { }

        public override string ProcessName => "BillingProcessStep1Result";

        public virtual PopulateToolBillingResult PopulateToolBillingProcessResult { get; set; }
        public virtual PopulateRoomBillingResult PopulateRoomBillingProcessResult { get; set; }
        public virtual PopulateStoreBillingResult PopulateStoreBillingProcessResult { get; set; }

        protected override void WriteLog()
        {
            AppendResult(PopulateToolBillingProcessResult);
            AppendResult(PopulateRoomBillingProcessResult);
            AppendResult(PopulateStoreBillingProcessResult);
        }
    }
}