using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Email.Criteria
{
    public class ByManager : IRecipientCriteria
    {
        public int SelectedManagerClientID { get; set; }

        public IEnumerable<MassEmailRecipient> GetRecipients()
        {
            return GroupEmailManager.GetEmailListByManagerID(SelectedManagerClientID);
        }
    }
}
