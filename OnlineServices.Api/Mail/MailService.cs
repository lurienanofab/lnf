using LNF.Models.Mail;
using Newtonsoft.Json;
using System;

namespace OnlineServices.Api.Mail
{
    public class MailService : ApiClient, IMailService
    {
        public string Get()
        {
            return Get("webapi/mail");
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
