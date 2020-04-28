namespace LNF.Reporting.Individual
{
    public abstract class ManagerUsageSummaryItem
    {
        public string Name { get; set; }
        public abstract string Members { get; }
        public string Sort { get; set; }
        public double NetCharge { get { return UsageCharge - Subsidy; } }
        public double UsageCharge { get; set; }
        public double Subsidy { get; set; }
    }
}
