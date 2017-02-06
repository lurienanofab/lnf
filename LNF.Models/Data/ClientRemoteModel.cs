namespace LNF.Models.Data
{
    public class ClientRemoteModel
    {
        public int ClientRemoteID { get; set; }
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public int RemoteClientID { get; set; }
        public string RemoteDisplayName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public bool Active { get; set; }
    }
}
