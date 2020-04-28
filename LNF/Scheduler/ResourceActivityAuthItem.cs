namespace LNF.Scheduler
{
    public class ResourceActivityAuthItem : IResourceActivityAuth
    {
        public int ResourceActivityAuthID { get; set; }
        public int ResourceID { get; set; }
        public int ActivityID { get; set; }
        public ClientAuthLevel UserAuth { get; set; }
        public ClientAuthLevel InviteeAuth { get; set; }
        public ClientAuthLevel StartEndAuth { get; set; }
        public ClientAuthLevel NoReservFenceAuth { get; set; }
        public ClientAuthLevel NoMaxSchedAuth { get; set; }
    }
}
