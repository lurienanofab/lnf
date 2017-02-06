using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Email.Criteria
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
