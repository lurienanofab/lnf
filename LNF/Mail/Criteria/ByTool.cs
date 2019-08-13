using LNF.Models.Mail;
using System.Collections.Generic;

namespace LNF.Mail.Criteria
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
