using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository.Data;

namespace LNF.Hooks
{
    abstract public class BeforeLogInHook : Hook<BeforeLogInHookContext, BeforeLogInHookResult>
    {
        abstract override protected void Execute();
    }

    public class BeforeLogInHookContext : HookContext
    {
        private string _Username;
        private string _Password;

        public string Username { get { return _Username; } }
        public string Password { get { return _Password; } }

        public BeforeLogInHookContext(string username, string password)
        {
            _Username = username;
            _Password = password;
        }
    }

    public class BeforeLogInHookResult : HookResult
    {
        public Client Client { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
