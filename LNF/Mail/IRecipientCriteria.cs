using LNF.Models.Mail;
using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IRecipientCriteria
    {
        IEnumerable<MassEmailRecipient> GetRecipients();
    }
}
