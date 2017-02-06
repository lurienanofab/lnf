using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Email.Criteria
{
    public class ByLab : IRecipientCriteria
    {
        public int[] SelectedLabs { get; set; }

        public IEnumerable<MassEmailRecipient> GetRecipients()
        {
            return GroupEmailManager.GetEmailListByInLab(SelectedLabs);
        }
    }
}
