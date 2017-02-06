using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository.Data;

namespace LNF.Hooks
{
    public abstract class AfterLogInHook : Hook<AfterLogInHookContext, AfterLogInHookResult>
    {
        abstract override protected void Execute();
    }

    public class AfterLogInHookContext : HookContext
    {
        private Client _LoggedInClient;
        private bool _IsKiosk;

        public Client LoggedInClient
        {
            get { return _LoggedInClient; }
        }

        public bool IsKiosk
        {
            get { return _IsKiosk; }
        }

        public AfterLogInHookContext(Client loggedInClient, bool isKiosk)
        {
            _LoggedInClient = loggedInClient;
            _IsKiosk = isKiosk;
        }
    }

    public class AfterLogInHookResult : HookResult
    {
        public Client Client { get; set; }
        public bool Redirect { get; set; }
        public string RedirectUrl { get; set; }
    }
}
