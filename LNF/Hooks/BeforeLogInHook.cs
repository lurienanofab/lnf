using LNF.Data;

namespace LNF.Hooks
{
    abstract public class BeforeLogInHook : Hook<BeforeLogInHookContext, BeforeLogInHookResult>
    {
        public BeforeLogInHook(IProvider provider) : base(provider) { }

        abstract override protected void Execute();
    }

    public class BeforeLogInHookContext : HookContext
    {
        public string Username { get; }
        public string Password { get; }

        public BeforeLogInHookContext(IProvider provider, string username, string password) : base(provider)
        {
            Username = username;
            Password = password;
        }
    }

    public class BeforeLogInHookResult : HookResult
    {
        public IClient Client { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
