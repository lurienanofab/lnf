using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Email.Criteria
{
    public class ByTool : IRecipientCriteria
    {
        public int[] SelectedResourceIDs { get; set; }

        public IEnumerable<MassEmailRecipient> GetRecipients()
        {
            return GroupEmailManager.GetEmailListByTools(SelectedResourceIDs);
        }
    }
}
