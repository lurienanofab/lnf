using LNF.Models.Mail;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class MassEmailManager : ApiClient, IMassEmailManager
    {
        public IEnumerable<MassEmailRecipient> GetRecipients(MassEmailRecipientArgs args)
        {
            return Post<List<MassEmailRecipient>>("webapi/mail/mass-email/recipient", args);
        }

        public int Send(MassEmailSendArgs args)
        {
            //var files = new FileCollection("attachments", attachments);
            return Post<int>("webapi/mail/mass-email/send", args);
        }
    }
}
