namespace LNF.Ordering
{
    public interface IApprover
    {
        bool Active { get; set; }
        string ApproverDisplayName { get; set; }
        int ApproverID { get; set; }
        int ClientID { get; set; }
        string DisplayName { get; set; }
        bool IsPrimary { get; set; }
    }
}