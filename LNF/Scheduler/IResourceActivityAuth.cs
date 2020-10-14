namespace LNF.Scheduler
{
    public interface IResourceActivityAuth
    {
        int ResourceActivityAuthID { get; set; }
        int ResourceID { get; set; }
        int ActivityID { get; set; }
        ClientAuthLevel UserAuth { get; set; }
        ClientAuthLevel InviteeAuth { get; set; }
        ClientAuthLevel StartEndAuth { get; set; }
        ClientAuthLevel NoReservFenceAuth { get; set; }
        ClientAuthLevel NoMaxSchedAuth { get; set; }
    }
}