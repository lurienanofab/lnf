using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public class AdministrativeHelper : ManagerBase
    {
        public AdministrativeHelper(ISession session) : base(session) { }

        public void SendEmailToDevelopers(string subject, string body)
        {
            if (ServiceProvider.Current.Email != null)
            {
                string from = "system@lnf.umich.edu";
                IEnumerable<string> to = GetEmailListByPrivilege(ClientPrivilege.Developer);
                ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.AdministrativeHelper.SendEmailToDevelopers(string subject, string body)", subject, body, from, to);
            }
        }

        public IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs)
        {
            var mgr = Session.ClientManager();
            var clients = mgr.FindByPrivilege(privs);
            IEnumerable<string> result = clients.Select(c => mgr.PrimaryEmail(c));
            return result;
        }
    }
}
