using System.Collections.Generic;
using System.Linq;

namespace LNF.Mail
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

        public string[] GetDistinctEmails()
        {
            var list = new List<string>();

            if (To != null && To.Count() > 0)
                list.AddRange(To);

            if (Cc != null && Cc.Count() > 0)
                list.AddRange(Cc);

            if (Bcc != null && Bcc.Count() > 0)
                list.AddRange(Bcc);

            var result = list.Distinct().ToArray();

            return result;
        }
    }
}
