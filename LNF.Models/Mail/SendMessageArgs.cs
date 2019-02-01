using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public class SendMessageArgs
    {
        public int ClientID { get; set; }
        public string Caller { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<string> To { get; set; }
        public IEnumerable<string> Cc { get; set; }
        public IEnumerable<string> Bcc { get; set; }
        public IEnumerable<string> Attachments { get; set; }
        public bool IsHtml { get; set; }
    }
}
