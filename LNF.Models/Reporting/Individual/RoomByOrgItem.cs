namespace LNF.Models.Reporting.Individual
{
    public class RoomByOrgItem
    {
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public decimal RoomCost { get; set; }
        public decimal MiscCarge { get; set; }
        public decimal SubsidyDiscount { get; set; }
    }
}
