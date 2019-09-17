namespace LNF.Models.Scheduler
{
    public interface IActivity
    {
        ActivityAccountType AccountType { get; set; }
        int ActivityID { get; set; }
        string ActivityName { get; set; }
        bool Chargeable { get; set; }
        string Description { get; set; }
        bool Editable { get; set; }
        int InviteeAuth { get; set; }
        ActivityInviteeType InviteeType { get; set; }
        bool IsActive { get; set; }
        bool IsFacilityDownTime { get; set; }
        bool IsRepair { get; }
        int ListOrder { get; set; }
        int NoMaxSchedAuth { get; set; }
        int NoReservFenceAuth { get; set; }
        int StartEndAuth { get; set; }
        int UserAuth { get; set; }
    }
}