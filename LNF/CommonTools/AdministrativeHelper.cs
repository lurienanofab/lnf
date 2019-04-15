using LNF.Models.Data;
using LNF.Repository;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public class AdministrativeHelper : ManagerBase, IAdministrativeHelper
    {
        public AdministrativeHelper(IProvider provider) : base(provider) { }

        public void SendEmailToDevelopers(string subject, string body)
        {
            SendEmail.SendDeveloperEmail("LNF.CommonTools.AdministrativeHelper.SendEmailToDevelopers", subject, body);
        }

        public IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs)
        {
            var clients = Provider.Data.Client.FindByPrivilege(privs);
            IEnumerable<string> result = clients.Select(c => c.Email);
            return result;
        }
    }
}
