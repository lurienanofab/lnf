namespace LNF.Models.Data
{
    public interface IClientRemote
    {
        int ClientRemoteID { get; set; }
        int ClientID { get; set; }
        string DisplayName { get; set; }
        int RemoteClientID { get; set; }
        string RemoteDisplayName { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
    }
}