using LNF.Impl.Repository.Data;
using LNF.Mail;
using System.Collections.Generic;

namespace LNF.Impl.Mail.Criteria
{
    public class ByManager : CriteriaBase
    {
        public int SelectedManagerClientID { get; set; }

        public override IEnumerable<MassEmailRecipient> GetRecipients()
        {
            var mgr = new GroupEmailManager(Session);
            return mgr.GetEmailListByManagerID(SelectedManagerClientID);
        }

        public override string GetGroupName()
        {
            return Session.Get<Client>(SelectedManagerClientID).DisplayName;
        }
    }
}
