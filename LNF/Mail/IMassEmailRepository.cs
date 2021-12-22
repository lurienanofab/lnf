using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IMassEmailRepository
    {
        IEnumerable<MassEmailRecipient> GetRecipients(MassEmailRecipientArgs args);
        SendMessageArgs CreateSendMessageArgs(MassEmailSendArgs args);
        IEnumerable<IInvalidEmail> GetInvalidEmails(bool? active = null);
        IInvalidEmail GetInvalidEmail(int emailId);
        int AddInvalidEmail(IInvalidEmail model);
        bool ModifyInvalidEmail(IInvalidEmail model);
        bool SetInvalidEmailActive(int emailId, bool value);
        bool DeleteInvalidEmail(int emailId);
        IRecipientCriteria CreateCriteria(IMassEmail massEmail);
    }
}
