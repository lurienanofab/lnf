using LNF.Models.Mail;
using System.Collections.Generic;
using System.Net.Mail;

namespace LNF.CommonTools
{
    public static class SendEmail
    {
        public static void Send(int clientId, string caller, string subject, string body, string from, IEnumerable<string> to = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, bool isHtml = true)
        {
            ServiceProvider.Current.Mail.SendMessage(new SendMessageArgs
            {
                ClientID = clientId,
                Caller = caller,
                Subject = subject,
                Body = body,
                From = SystemEmail,
                To = to,
                Cc = cc,
                Bcc = bcc,
                IsHtml = isHtml
            });
        }

        public static void Email(string fromAddr, string toAddr, bool ccSelf, string subject, string body, bool isHtml = true)
        {
            string[] to = new string[] { toAddr };
            string[] cc = (ccSelf) ? new string[] { fromAddr } : null;
            Send(0, "LNF.CommonTools.SendEmail.Email", subject, body, fromAddr, to, cc, isHtml: isHtml);
        }

        public static void SendSystemEmail(string caller, string subject, string body = "", IEnumerable<string> to = null, bool isHtml = true)
        {
            Send(0, caller, subject, body, SystemEmail, to, isHtml: isHtml);
        }

        public static void SendDeveloperEmail(string caller, string subject, string body = "")
        {
            SendSystemEmail(caller, subject, body, DeveloperEmails, false);
        }

        public static string SystemEmail => Utility.GetGlobalSetting("SystemEmail");

        public static string[] DeveloperEmails => Utility.GetGlobalSetting("DeveloperEmails").Split(',');

        public static string CompanyName => Utility.GetGlobalSetting("CompanyName");

        public static bool ValidateEmail(string addr)
        {
            try
            {
                var ma = new MailAddress(addr);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
