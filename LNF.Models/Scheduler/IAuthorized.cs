namespace LNF.Models.Scheduler
{
    public interface IAuthorized
    {
        int ClientID { get; set; }
        ClientAuthLevel AuthLevel { get; set; }
        bool HasAuth(ClientAuthLevel auths);
        bool IsEveryone();
    }
}
