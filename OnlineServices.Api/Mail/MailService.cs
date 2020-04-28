using LNF.Data;
using LNF.Mail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class MailService : ApiClient, IMailService
    {
        public IMassEmailRepository MassEmail { get; }
        public IAttachmentUtility Attachment { get; }

        public MailService(IMassEmailRepository massEmail, IAttachmentUtility attachment)
        {
            MassEmail = massEmail;
            Attachment = attachment;
        }

        public IMessage GetMessage(int messageId)
        {
            return Get<Message>("webapi/mail/message/{messageId}", UrlSegments(new { messageId }));
        }

        public IEnumerable<IMessage> GetMessages(DateTime sd, DateTime ed, int clientId = 0)
        {
            return Get<List<Message>>("webapi/mail/message", QueryStrings(new { sd, ed, clientId }));
        }

        public IEnumerable<IRecipient> GetRecipients(int messageId)
        {
            return Get<List<Recipient>>("webapi/mail/message/{messageId}/recipient", UrlSegments(new { messageId }));
        }

        public void SendMessage(SendMessageArgs args)
        {
            var content = Post("webapi/mail/message", args);
            var errmsg = JsonConvert.DeserializeObject(content).ToString();
            if (!string.IsNullOrEmpty(errmsg))
                throw new Exception(errmsg);
        }

        public IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs)
        {
            throw new NotImplementedException();
        }
    }
}
