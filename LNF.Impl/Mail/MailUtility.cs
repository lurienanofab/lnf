using LNF.Models.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace LNF.Impl.Mail
{
    public static class MailUtility
    {
        public static EmailServiceElement Configuration { get; }

        static MailUtility()
        {
            var section = ConfigurationManager.GetSection("lnf/provider") as ServiceProviderSection;

            if (section == null)
                throw new Exception("Missing required configuration section: lnf/provider");

            Configuration = section.Email;
        }

        public static void Send(SendMessageArgs args)
        {
            MailMessage mm = new MailMessage { From = GetFromAddress(args) };

            AddAddresses(mm.To, args.To);
            AddAddresses(mm.CC, args.Cc);
            AddAddresses(mm.Bcc, args.Bcc);

            mm.Subject = args.Subject;
            mm.Body = args.Body;
            mm.IsBodyHtml = args.IsHtml;

            AddAttachments(mm.Attachments, args.Attachments);

            var smtp = GetSmtpClient();

            smtp.Send(mm);
        }

        public static SmtpClient GetSmtpClient()
        {
            var client = new SmtpClient(Configuration.Host, Configuration.Port);

            if (!string.IsNullOrEmpty(Configuration.Username) && !string.IsNullOrEmpty(Configuration.Password))
            {
                client.Credentials = new NetworkCredential(Configuration.Username, Configuration.Password);
                client.EnableSsl = Configuration.EnableSsl;
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
                    col.Add(new Attachment(att));
                }
            }
        }
    }
}