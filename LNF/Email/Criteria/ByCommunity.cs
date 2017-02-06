using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Email.Criteria
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
