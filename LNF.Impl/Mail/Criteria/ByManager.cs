using LNF.Impl.Repository.Data;
using LNF.Mail;
using System.Collections.Generic;

namespace LNF.Impl.Mail.Criteria
{
    public class ByManager : CriteriaBase
    {
        public int SelectedManagerClientID { get; set; }

        protected override IEnumerable<MassEmailRecipient> GetRecipients()
        {
            var mgr = new GroupEmailManager(Session);
            return mgr.GetEmailListByManagerID(SelectedManagerClientID);
        }

        protected override string GetGroupName()
        {
            return Session.Get<Client>(SelectedManagerClientID).DisplayName;
        }
    }
}
