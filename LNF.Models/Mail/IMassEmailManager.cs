using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public interface IMassEmailManager
    {
        IEnumerable<MassEmailRecipient> GetRecipients(MassEmailRecipientArgs args);
        int Send(MassEmailSendArgs args);
    }
}
