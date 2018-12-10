namespace LNF.Models.Billing.Process
{
    public class PopulateSubsidyBillingProcessResult : ProcessResult
    {
        public PopulateSubsidyBillingProcessResult() : base("PopulateSubsidyBilling") { }

        public string Command { get; set; }
    }
}
