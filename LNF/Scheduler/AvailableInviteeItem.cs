using LNF.Models.Data;

namespace LNF.Scheduler
{
    public class AvailableInviteeItem
    {
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string DisplayName => ClientItem.GetDisplayName(LName, FName);
        

        public static AvailableInviteeItem Create(int clientId, string lname, string fname)
        {
            return new AvailableInviteeItem()
            {
                ClientID = clientId,
                LName = lname,
                FName = fname
            };
        }
    }
}
