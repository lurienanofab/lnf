using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IMassEmailRepository
    {
        IEnumerable<MassEmailRecipient> GetRecipients(MassEmailRecipientArgs args);
        int Send(IMailService svc, MassEmailSendArgs args);
        IEnumerable<IInvalidEmail> GetInvalidEmails(bool? active = null);
        IInvalidEmail GetInvalidEmail(int emailId);
        int AddInvalidEmail(IInvalidEmail model);
        bool ModifyInvalidEmail(IInvalidEmail model);
        bool SetInvalidEmailActive(int emailId, bool value);
        IRecipientCriteria GetCriteria(IMassEmail massEmail);
    }
}
