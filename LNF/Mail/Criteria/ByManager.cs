using LNF.Models.Mail;
using System.Collections.Generic;

namespace LNF.Mail.Criteria
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
