namespace LNF.Scheduler
{
    public class AvailableInviteeItem
    {
        public int ClientID { get; set; }
        public string DisplayName { get; set; }

        public static AvailableInviteeItem Create(int clientId, string displayName)
        {
            return new AvailableInviteeItem()
            {
                ClientID = clientId,
                DisplayName = displayName
            };
        }
    }
}
