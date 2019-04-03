namespace LNF.Models.Ordering
{
    public class ApproverItem : IApprover
    {
        public int ApproverID { get; set; }
        public string ApproverDisplayName { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public bool IsPrimary { get; set; }
        public bool Active { get; set; }
    }
}
