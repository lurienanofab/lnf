namespace LNF.Scheduler
{
    public interface IReservationInviteeItem : IReservationItem
    {
        int InviteeID { get; set; }
        string InviteeLName { get; set; }
        string InviteeFName { get; set; }
        string InviteeDisplayName { get; }
    }
}
