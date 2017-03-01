using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Hooks;

namespace LNF.Impl.Hooks
{
    public class UniversalPasswordHook : BeforeLogInHook
    {
        protected override void Execute()
        {
            if (!string.IsNullOrEmpty(Providers.DataAccess.UniversalPassword) && Context.Password == Providers.DataAccess.UniversalPassword)
            {
                Client c = DA.Current.Query<Client>().FirstOrDefault(x => x.UserName == Context.Username);
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
