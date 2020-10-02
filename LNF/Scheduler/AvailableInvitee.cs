using LNF.Data;

namespace LNF.Scheduler
{
    public class AvailableInvitee
    {
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public override string ToString() => $"{DisplayName} [{ClientID}]";
    }
}
