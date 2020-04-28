using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Mail
{
    public interface IMailService
    {
        IMassEmailRepository MassEmail { get; }
        IAttachmentUtility Attachment { get; }
        void SendMessage(SendMessageArgs args);
        IEnumerable<IMessage> GetMessages(DateTime sd, DateTime ed, int clientId);
        IMessage GetMessage(int messageId);
        IEnumerable<IRecipient> GetRecipients(int messageId);
        IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs);
    }
}
