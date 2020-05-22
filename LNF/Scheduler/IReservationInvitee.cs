using LNF.Data;

namespace LNF.Scheduler
{
    public interface IReservationInvitee : IReservation
    {
        int InviteeID { get; set; }
        bool InviteeActive { get; set; }
        string InviteeLName { get; set; }
        string InviteeFName { get; set; }
        ClientPrivilege InviteePrivs { get; set; }
        bool Removed { get; set; }
    }
}