using LNF.Hooks;

namespace LNF.Impl.Hooks
{
    public class UniversalPasswordHook : BeforeLogInHook
    {
        public UniversalPasswordHook(IProvider provider) : base(provider) { }

        protected override void Execute()
        {
            if (!string.IsNullOrEmpty(Configuration.Current.DataAccess.UniversalPassword) && Context.Password == Configuration.Current.DataAccess.UniversalPassword)
            {
                var c = Provider.Data.Client.GetClient(Context.Username);
                if (c != null)
                {
                    Result.Client = c;
                    Result.IsLoggedIn = true;
                    return;
                }
            }

            Result.Client = null;
            Result.IsLoggedIn = false;
        }
    }
}
