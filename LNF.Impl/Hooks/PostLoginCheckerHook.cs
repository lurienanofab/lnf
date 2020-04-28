using LNF.Data;
using LNF.Hooks;

namespace LNF.Impl.Hooks
{
    public class PostLoginCheckerHook : AfterLogInHook
    {
        public PostLoginCheckerHook(IProvider provider) : base(provider) { }

        protected override void Execute()
        {
            if (Context.LoggedInClient == null) return;

            //2008-04-21 For all lab users to view CNF ethical videos
            if (Context.LoggedInClient.HasPriv(ClientPrivilege.LabUser))
            {
                if (!Context.IsKiosk)
                {
                    if (Context.LoggedInClient.ClientID < 890) //2008-05-19 requested by sandrine not to include new users
                    {
                        if (!Context.HasWatchedEthicalVideo())
                        {
                            Result.Redirect = true;
                            Result.RedirectUrl = "ViewVideo.aspx";
                            return;
                        }
                    }
                }
            }

            if (Context.LoggedInClient.ClientID >= 1328 && Context.LoggedInClient.ClientID <= 1335)
            {
                if (!Context.IsKiosk)
                {
                    if (!Context.HasTakenSafetyTest())
                    {
                        Result.Redirect = true;
                        Result.RedirectUrl = "TestHF.aspx";
                        return;
                    }
                }
            }
        }
    }
}
