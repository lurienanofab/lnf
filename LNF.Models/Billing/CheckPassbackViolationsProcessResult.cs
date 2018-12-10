namespace LNF.Models.Billing
{
    public class CheckPassbackViolationsProcessResult : ProcessResult
    {
        public CheckPassbackViolationsProcessResult() : base("CheckPassbackViolations") { }

        public int TotalPassbackViolations { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"TotalPassbackViolations: {TotalPassbackViolations}");
        }
    }
}
