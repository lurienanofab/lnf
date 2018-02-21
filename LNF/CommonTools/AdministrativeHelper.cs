using LNF.Models.Data;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public static class AdministrativeHelper
    {
        public static void SendEmailToDevelopers(string subject, string body)
        {
            if (Providers.Email != null)
            {
                string from = "system@lnf.umich.edu";
                IEnumerable<string> to = GetEmailListByPrivilege(ClientPrivilege.Developer);
                Providers.Email.SendMessage(0, "LNF.CommonTools.AdministrativeHelper.SendEmailToDevelopers(string subject, string body)", subject, body, from, to);
            }
        }

        public static IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs)
        {
            IEnumerable<string> result = Client.FindByPrivilege(privs).Select(c => c.PrimaryEmail());
            return result;
        }
    }
}
