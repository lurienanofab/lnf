using System;
using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public interface IMailService
    {
        void SendMessage(SendMessageArgs args);
        IEnumerable<IMessage> GetMessages(DateTime sd, DateTime ed, int clientId);
        IMessage GetMessage(int messageId);
        IEnumerable<IRecipient> GetRecipients(int messageId);
    }
}
