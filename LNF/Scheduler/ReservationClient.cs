namespace LNF.Scheduler
{
    public class ReservationClient
    {
        public int ClientID { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public bool IsReserver { get; set; }
        public bool IsInvited { get; set; }
        public bool InLab { get; set; }
        public ClientAuthLevel UserAuth { get; set; }
    }
}
