namespace LNF.Repository.Ordering
{
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
