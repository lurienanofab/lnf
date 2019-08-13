using LNF.Models.Mail;
using System.Collections.Generic;

namespace LNF.Mail.Criteria
{
    public class ByCommunity : IRecipientCriteria
    {
        public int SelectedCommunities { get; set; }

        public IEnumerable<MassEmailRecipient> GetRecipients()
        {
            return GroupEmailManager.GetEmailListByCommunity(SelectedCommunities);
        }
    }
}
