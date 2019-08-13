using System;
using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public class MassEmailSendArgs
    {
        public string Group { get; set; }
        public IEnumerable<int> Values { get; set; }
        public Guid Attachments { get; set; }
        public string From { get; set; }
        public string CC { get; set; }
        public string DisplayName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int ClientID { get; set; }
        public string Caller { get; set; }
    }
}
