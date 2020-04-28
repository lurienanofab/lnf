using LNF.Mail;
using System;

namespace OnlineServices.Api.Mail
{
    public class Message : IMessage
    {
        public int MessageID { get; set; }
        public int ClientID { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Error { get; set; }
        public string Caller { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? SentOn { get; set; }
    }
}
