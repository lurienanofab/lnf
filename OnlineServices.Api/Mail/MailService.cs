using LNF.Models.Mail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class MailService : ApiClient, IMailService
    {
        public IMassEmailManager MassEmail { get; }
        public IAttachmentManager Attachment { get; }

        public MailService(IMassEmailManager massEmail, IAttachmentManager attachment)
        {
            MassEmail = massEmail;
            Attachment = attachment;
        }

        public IMessage GetMessage(int messageId)
        {
            return Get<MessageItem>("webapi/mail/message/{messageId}", UrlSegments(new { messageId }));
        }

        public IEnumerable<IMessage> GetMessages(DateTime sd, DateTime ed, int clientId = 0)
        {
            return Get<List<MessageItem>>("webapi/mail/message", QueryStrings(new { sd, ed, clientId }));
        }

        public IEnumerable<IRecipient> GetRecipients(int messageId)
        {
            return Get<List<RecipientItem>>("webapi/mail/message/{messageId}/recipient", UrlSegments(new { messageId }));
        }

        public void SendMessage(SendMessageArgs args)
        {
            var content = Post("webapi/mail/message", args);
            var errmsg = JsonConvert.DeserializeObject(content).ToString();
            if (!string.IsNullOrEmpty(errmsg))
                throw new Exception(errmsg);
        }
    }
}
