using LNF.Data;
using LNF.Mail;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class MailService : ApiClient, IMailService
    {
        public IMassEmailRepository MassEmail { get; }
        public IAttachmentUtility Attachment { get; }

        internal MailService(IRestClient rc) : base(rc)
        {
            MassEmail = new MassEmailRepository(rc);
            Attachment = new AttachmentUtility(rc);
        }

        public Message GetMessage(int messageId)
        {
            return Get<Message>("webapi/mail/message/{messageId}", UrlSegments(new { messageId }));
        }

        public IEnumerable<Message> GetMessages(DateTime sd, DateTime ed, int clientId = 0)
        {
            return Get<List<Message>>("webapi/mail/message", QueryStrings(new { sd, ed, clientId }));
        }

        public IEnumerable<Recipient> GetRecipients(int messageId)
        {
            return Get<List<Recipient>>("webapi/mail/message/{messageId}/recipient", UrlSegments(new { messageId }));
        }

        public int SendMassEmail(MassEmailSendArgs args)
        {
            return Post<int>("webapi/mail/send/mass-email", args);
        }

        public void SendMessage(SendMessageArgs args)
        {
            var content = Post("webapi/mail/send/message", args);
            var errmsg = JsonConvert.DeserializeObject(content).ToString();
            if (!string.IsNullOrEmpty(errmsg))
                throw new Exception(errmsg);
        }

        public IEnumerable<string> GetEmailListByPrivilege(ClientPrivilege privs)
        {
            return Get<List<string>>("webapi/mail/message/recipient", QueryStrings(new { privs = Convert.ToInt32(privs) }));
        }
    }
}
