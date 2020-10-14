namespace LNF.Scheduler
{
    public interface IActivity
    {
        int ActivityID { get; set; }
        string ActivityName { get; set; }
        int ListOrder { get; set; }
        bool Chargeable { get; set; }
        bool Editable { get; set; }
        ActivityAccountType AccountType { get; set; }
        ClientAuthLevel UserAuth { get; set; }
        ActivityInviteeType InviteeType { get; set; }
        ClientAuthLevel InviteeAuth { get; set; }
        ClientAuthLevel StartEndAuth { get; set; }
        ClientAuthLevel NoReservFenceAuth { get; set; }
        ClientAuthLevel NoMaxSchedAuth { get; set; }
        string Description { get; set; }
        bool IsActive { get; set; }
        bool IsFacilityDownTime { get; set; }
        bool IsRepair { get; }
    }
}