using LNF.Mail;
using RestSharp;
using System.Collections.Generic;

namespace OnlineServices.Api.Mail
{
    public class MassEmailRepository : ApiClient, IMassEmailRepository
    {
        internal MassEmailRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<MassEmailRecipient> GetRecipients(MassEmailRecipientArgs args)
        {
            return Post<List<MassEmailRecipient>>("webapi/mail/mass-email/recipient", args);
        }

        public SendMessageArgs CreateSendMessageArgs(MassEmailSendArgs args)
        {
            //var files = new FileCollection("attachments", attachments);
            return Post<SendMessageArgs>("webapi/mail/mass-email/send-message-args", args);
        }

        public IEnumerable<IInvalidEmail> GetInvalidEmails(bool? active = null)
        {
            return Get<List<InvalidEmailItem>>("webapi/mail/mass-email/invalid-email", QueryStrings(new { active }));
        }

        public IInvalidEmail GetInvalidEmail(int emailId)
        {
            return Get<InvalidEmailItem>("webapi/mail/mass-email/invalid-email/{emailId}", UrlSegments(new { emailId }));
        }

        public int AddInvalidEmail(IInvalidEmail model)
        {
            return Post<int>("webapi/mail/mass-email/invalid-email/add", model);
        }

        public bool ModifyInvalidEmail(IInvalidEmail model)
        {
            return Post<bool>("webapi/mail/mass-email/invalid-email/modify", model);
        }

        public bool SetInvalidEmailActive(int emailId, bool value)
        {
            //var url = $"webapi/mail/mass-email/invalid-email/{emailId}/active?value={value.ToString().ToLower()}";
            //return Put(url, parameters: null);
            return Put("webapi/mail/mass-email/invalid-email/{emailId}/active", UrlSegments(new { emailId }) & QueryStrings(new { value }));
        }

        public bool DeleteInvalidEmail(int emailId)
        {
            int delete = Delete("webapi/mail/mass-email/invalid-email/{emailId}", UrlSegments(new { emailId }));
            return delete == 1;
        }

        public IRecipientCriteria CreateCriteria(IMassEmail massEmail)
        {
            return Post<RecipientCriteria>("webapi/mail/mass-email/criteria", massEmail);
        }
    }
}
