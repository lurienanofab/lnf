namespace LNF.Ordering
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

    public enum TrackingCheckpoints
    {
        DraftCreated = 1,
        SentForApproval = 2,
        ManuallyProcessed = 3,
        Approved = 4,
        Rejected = 5,
        Claimed = 6,
        Ordered = 7,
        MarkedComplete = 8,
        Deleted = 9,
        SentToPurchaser = 10,
        Cancelled = 11,
        Modified = 12
    }
}
