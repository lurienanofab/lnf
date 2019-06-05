namespace LNF.Models.Scheduler
{
    public interface IResourceActivityAuth
    {
        int ActivityID { get; set; }
        ClientAuthLevel InviteeAuth { get; set; }
        ClientAuthLevel NoMaxSchedAuth { get; set; }
        ClientAuthLevel NoReservFenceAuth { get; set; }
        int ResourceActivityAuthID { get; set; }
        int ResourceID { get; set; }
        ClientAuthLevel StartEndAuth { get; set; }
        ClientAuthLevel UserAuth { get; set; }
    }
}