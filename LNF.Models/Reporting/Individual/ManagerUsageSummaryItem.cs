namespace LNF.Models.Reporting.Individual
{
    public abstract class ManagerUsageSummaryItem
    {
        public string Name { get; set; }
        public string Sort { get; set; }
        public double NetCharge { get { return UsageCharge - Subsidy; } }
        public double UsageCharge { get; set; }
        public double Subsidy { get; set; }
    }
}
