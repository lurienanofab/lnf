namespace LNF.Billing.Apportionment.Models
{
    public class ApportionmentEntry
    {
        public int RoomID { get; set; }
        public double TotalEntries { get; set; }
        public decimal PhysicalDays { get; set; }
    }
}
