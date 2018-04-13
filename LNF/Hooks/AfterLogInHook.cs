using LNF.Models.Data;
using LNF.Repository.Data;
using LNF.Repository;

namespace LNF.Hooks
{
    public abstract class AfterLogInHook : Hook<AfterLogInHookContext, AfterLogInHookResult>
    {
        abstract override protected void Execute();
    }

    public class AfterLogInHookContext : HookContext
    {
        private Client _client;
        private ClientItem _LoggedInClient;
        private bool _IsKiosk;

        public ClientItem LoggedInClient
        {
            get { return _LoggedInClient; }
        }

        public bool IsKiosk
        {
            get { return _IsKiosk; }
        }

        public AfterLogInHookContext(ClientItem loggedInClient, bool isKiosk)
        {
            _client = DA.Current.Single<Client>(loggedInClient.ClientID);
            _LoggedInClient = loggedInClient;
            _IsKiosk = isKiosk;
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
