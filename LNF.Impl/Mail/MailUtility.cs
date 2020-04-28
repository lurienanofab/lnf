using LNF.Mail;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace LNF.Impl.Mail
{
    public static class MailUtility
    {
        private readonly static EmailServiceElement _config;

        static MailUtility()
        {
            _config = Configuration.Current.Email;
        }

        public static void Send(SendMessageArgs args)
        {
            using (var smtp = GetSmtpClient())
            using (var mm = new MailMessage { From = GetFromAddress(args) })
            {
                AddAddresses(mm.To, args.To);
                AddAddresses(mm.CC, args.Cc);
                AddAddresses(mm.Bcc, args.Bcc);

                mm.Subject = args.Subject;
                mm.Body = args.Body;
                mm.IsBodyHtml = args.IsHtml;

                AddAttachments(mm.Attachments, args.Attachments);

                smtp.Send(mm);
            }
        }

        public static SmtpClient GetSmtpClient()
        {
            var client = new SmtpClient(_config.Host, _config.Port);

            if (!string.IsNullOrEmpty(_config.Username) && !string.IsNullOrEmpty(_config.Password))
            {
                client.Credentials = new NetworkCredential(_config.Username, _config.Password);
                client.EnableSsl = _config.EnableSsl;
            }

            return client;
        }

        public static MailAddress GetFromAddress(SendMessageArgs args)
        {
            MailAddress result;

            if (!string.IsNullOrEmpty(args.DisplayName))
                result = new MailAddress(args.From, args.DisplayName);
            else
                result = new MailAddress(args.From);

            return result;
        }

        public static void AddAddresses(MailAddressCollection col, IEnumerable<string> addrs)
        {
            if (addrs != null)
            {
                foreach (var addr in addrs)
                {
                    col.Add(new MailAddress(addr));
                }
            }
        }

        public static void AddAttachments(AttachmentCollection col, IEnumerable<string> atts)
        {
            if (atts != null)
            {
                foreach (var att in atts)
                {
                    col.Add(new global::System.Net.Mail.Attachment(att));
                }
            }
        }
    }
}