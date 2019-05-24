using LNF.Models.Mail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class MailService : ApiClient, IMailService
    {
        public IMessage GetMessage(int messageId)
        {
            return Get<MessageItem>("webapi/mail/message/{messageId}", UrlSegments(new { messageId }));
        }

        public IEnumerable<IMessage> GetMessages(DateTime sd, DateTime ed, int clientId = 0)
        {
            return Get<List<MessageItem>>("webapi/mail/message/{messageId}", QueryStrings(new { sd, ed, clientId }));
        }

        public IEnumerable<IRecipient> GetRecipients(int messageId)
        {
            return Get<List<RecipientItem>>("message/{messageId}/recipient", UrlSegments(new { messageId }));
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
