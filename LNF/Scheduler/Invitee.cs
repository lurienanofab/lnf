using LNF.Data;

namespace LNF.Scheduler
{
    public class Invitee
    {
        public int ReservationID { get; set; }
        public int InviteeID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public bool Removed { get; set; }
        public override string ToString() => $"{DisplayName} [{InviteeID}]";
    }
}
