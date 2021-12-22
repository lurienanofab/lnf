using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IMailService
    {
        IMassEmailRepository MassEmail { get; }
        IAttachmentUtility Attachment { get; }
        int SendMassEmail(MassEmailSendArgs args);
        void SendMessage(SendMessageArgs args);
        IEnumerable<Message> GetMessages(DateTime sd, DateTime ed, int clientId);
        Message GetMessage(int messageId);
        IEnumerable<Recipient> GetRecipients(int messageId);
        IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs);
    }
}
