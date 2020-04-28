using LNF.Data;

namespace LNF.Scheduler
{
    public interface IAuthorized : IPrivileged
    {
        ClientAuthLevel AuthLevel { get; set; }
        bool HasAuth(ClientAuthLevel auths);
        bool IsEveryone();
    }
}
