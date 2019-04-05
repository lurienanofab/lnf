using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Hooks
{
    public abstract class AfterLogInHook : Hook<AfterLogInHookContext, AfterLogInHookResult>
    {
        abstract override protected void Execute();
    }

    public class AfterLogInHookContext : HookContext
    {
        private Client _client;

        public IClient LoggedInClient { get; }

        public bool IsKiosk { get; }

        public AfterLogInHookContext(IClient loggedInClient, bool isKiosk)
        {
            _client = DA.Current.Single<Client>(loggedInClient.ClientID);
            LoggedInClient = loggedInClient;
            IsKiosk = isKiosk;
        }

        public bool HasTakenSafetyTest()
        {
            return _client.HasTakenSafetyTest();
        }

        public bool HasWatchedEthicalVideo()
        {
            return _client.HasWatchedEthicalVideo();
        }
    }

    public class AfterLogInHookResult : HookResult
    {
        public Client Client { get; set; }
        public bool Redirect { get; set; }
        public string RedirectUrl { get; set; }
    }
}
