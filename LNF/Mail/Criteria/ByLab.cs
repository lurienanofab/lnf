using LNF.Models.Mail;
using System.Collections.Generic;

namespace LNF.Mail.Criteria
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
