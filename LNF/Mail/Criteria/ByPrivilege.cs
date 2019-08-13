using LNF.Models.Mail;
using System.Collections.Generic;

namespace LNF.Mail.Criteria
{
    public class ByPrivilege : IRecipientCriteria
    {
        public int SelectedPrivileges { get; set; }

        public IEnumerable<MassEmailRecipient> GetRecipients()
        {
            return GroupEmailManager.GetEmailListByPrivilege(SelectedPrivileges);
        }
    }
}
