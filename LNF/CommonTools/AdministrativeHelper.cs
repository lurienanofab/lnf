using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public class AdministrativeHelper : ManagerBase, IAdministrativeHelper
    {
        protected IClientManager ClientManager { get; }

        public AdministrativeHelper(ISession session, IClientManager clientManager) : base(session)
        {
            ClientManager = clientManager;
        }

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
            var clients = ClientManager.FindByPrivilege(privs);
            IEnumerable<string> result = clients.Select(c => ClientManager.PrimaryEmail(c));
            return result;
        }
    }
}
