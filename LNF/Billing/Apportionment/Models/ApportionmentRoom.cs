namespace LNF.Billing.Apportionment.Models
{
    public class ApportionmentRoom
    {
        public int RoomID { get; set; }
        public int? ParentID { get; set; }
        public string RoomName { get; set; }
        public string RoomDisplayName { get; set; }
        public bool Billable { get; set; }
        public bool ApportionDailyFee { get; set; }
        public bool ApportionEntryFee { get; set; }
        public bool Active { get; set; }
    }
}
