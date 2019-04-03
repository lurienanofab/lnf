using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Hooks;
using LNF.Models.Data;

namespace LNF.Impl.Hooks
{
    public class UniversalPasswordHook : BeforeLogInHook
    {
        protected override void Execute()
        {
            if (!string.IsNullOrEmpty(ServiceProvider.Current.DataAccess.UniversalPassword) && Context.Password == ServiceProvider.Current.DataAccess.UniversalPassword)
            {
                var c = DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.UserName == Context.Username);
                if (c != null)
                {
                    Result.Client = c.CreateModel<IClient>();
                    Result.IsLoggedIn = true;
                    return;
                }
            }

            Result.Client = null;
            Result.IsLoggedIn = false;
        }
    }
}
