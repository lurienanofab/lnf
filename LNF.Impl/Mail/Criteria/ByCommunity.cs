using LNF.Mail;
using System.Collections.Generic;

namespace LNF.Impl.Mail.Criteria
{
    public class ByCommunity : CriteriaBase
    {
        public int SelectedCommunities { get; set; }

        public override IEnumerable<MassEmailRecipient> GetRecipients()
        {
            var mgr = new GroupEmailManager(Session);
            return mgr.GetEmailListByCommunity(SelectedCommunities);
        }

        public override string GetGroupName()
        {
            return string.Join(", ", SelectedCommunities);
        }
    }
}
