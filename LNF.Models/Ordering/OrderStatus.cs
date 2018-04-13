namespace LNF.Models.Ordering
{
    public enum OrderStatus
    {
        Draft = 1,
        AwaitingApproval = 2,
        Approved = 3,
        Ordered = 4,
        Completed = 5,
        Cancelled = 6,
        ProcessedManually = 7
    }
}
