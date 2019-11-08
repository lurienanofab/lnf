using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public interface IMassEmailManager
    {
        IEnumerable<MassEmailRecipient> GetRecipients(MassEmailRecipientArgs args);
        int Send(MassEmailSendArgs args);
        IEnumerable<IInvalidEmail> GetInvalidEmails(bool? active = null);
        IInvalidEmail GetInvalidEmail(int emailId);
        int AddInvalidEmail(IInvalidEmail model);
        bool ModifyInvalidEmail(IInvalidEmail model);
        bool SetInvalidEmailActive(int emailId, bool value);
    }
}
