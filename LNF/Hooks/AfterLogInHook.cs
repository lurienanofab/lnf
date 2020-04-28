using LNF.Data;

namespace LNF.Hooks
{
    public abstract class AfterLogInHook : Hook<AfterLogInHookContext, AfterLogInHookResult>
    {
        public AfterLogInHook(IProvider provider) : base(provider) { }

        abstract override protected void Execute();
    }

    public class AfterLogInHookContext : HookContext
    {
        private IClient _client;

        public IClient LoggedInClient { get; }

        public bool IsKiosk { get; }

        public AfterLogInHookContext(IProvider provider, IClient loggedInClient, bool isKiosk) : base(provider)
        {
            _client = Provider.Data.Client.GetClient(loggedInClient.ClientID);
            LoggedInClient = loggedInClient;
            IsKiosk = isKiosk;
        }

        public bool HasTakenSafetyTest() => _client.IsSafetyTest;

        public bool HasWatchedEthicalVideo() => _client.IsChecked;
    }

    public class AfterLogInHookResult : HookResult
    {
        public IClient Client { get; set; }
        public bool Redirect { get; set; }
        public string RedirectUrl { get; set; }
    }
}
