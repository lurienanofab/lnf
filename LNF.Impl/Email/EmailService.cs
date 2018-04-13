using LNF.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace LNF.Impl.Email
{
    public class EmailService : IEmailService
    {
        private GroupUtility _GroupUtility;
        private EmailServiceElement _config;

        public EmailService()
        {
            _GroupUtility = new GroupUtility(this);
            _config = ServiceProvider.GetConfigurationSection().Email;
        }

        public string Host => _config.Host;
        public int Port => _config.Port;
        public string Username => _config.Username;
        public string Password => _config.Password;
        public bool EnableSsl => _config.EnableSsl;
        public bool Log => _config.Log;

        public string CompanyName
        {
            get { return "LNF"; }
        }

        public IEmailGroupUtility GroupUtility
        {
            get { return _GroupUtility; }
        }

        public bool ValidateEmail(string addr)
        {
            try
            {
                MailAddress ma = new MailAddress(addr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SendMessage(int clientId, string caller, string subject, string body, string from, IEnumerable<string> to = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, IEnumerable<string> attachments = null, bool isHtml = false)
        {
            var args = new SendMessageArgs()
            {
                ClientID = clientId,
                Caller = caller,
                Subject = subject,
                Body = body,
                From = from,
                DisplayName = string.Empty,
                To = to,
                Cc = cc,
                Bcc = bcc,
                Attachments = attachments,
                IsHtml = isHtml
            };

            SendMessage(args);
        }

        public void SendMessage(SendMessageArgs args)
        {
            Validate(args);

            MailMessage mm = new MailMessage();
            mm.Subject = args.Subject;
            mm.Body = args.Body;
            mm.From = GetFromAddress(args);
            AddAddresses(mm.To, args.To);
            AddAddresses(mm.CC, args.Cc);
            AddAddresses(mm.Bcc, args.Bcc);
            AddAttachments(mm.Attachments, args.Attachments);
            mm.IsBodyHtml = args.IsHtml;

            IEmailEnvelope env = Envelope.Create(args.ClientID, args.Caller, mm);

            env.Send();
        }

        private void Validate(SendMessageArgs args)
        {
            if (string.IsNullOrEmpty(args.From))
                throw new Exception("From address is required.");

            int recipientCount = 0;
            recipientCount += (args.To == null) ? 0 : args.To.Count();
            recipientCount += (args.Cc == null) ? 0 : args.Cc.Count();
            recipientCount += (args.Bcc == null) ? 0 : args.Bcc.Count();

            if (recipientCount == 0)
                throw new Exception("At least one recipient is required.");
        }

        public MailAddress GetFromAddress(SendMessageArgs args)
        {
            if (!string.IsNullOrEmpty(args.DisplayName))
                return new MailAddress(args.From, args.DisplayName);
            else
                return new MailAddress(args.From);
        }

        public void AddAttachments(AttachmentCollection collection, IEnumerable<string> attachments)
        {
            if (attachments == null) return;
            if (attachments.Count() == 0) return;

            foreach (string a in attachments)
            {
                if (!string.IsNullOrEmpty(a))
                {
                    Attachment att = TryGetAttachment(a);
                    if (att != null)
                        collection.Add(att);
                }
            }
        }

        public void AddAddresses(MailAddressCollection collection, IEnumerable<string> addresses)
        {
            if (addresses == null) return;
            if (addresses.Count() == 0) return;

            foreach (string a in addresses)
            {
                if (!string.IsNullOrEmpty(a))
                {
                    MailAddress addr = TryGetMailAddress(a);
                    if (addr != null)
                        collection.Add(addr);
                }
            }
        }

        public Attachment TryGetAttachment(string att)
        {
            try
            {
                return new Attachment(att);
            }
            catch
            {
                return null;
            }
        }

        public MailAddress TryGetMailAddress(string addr)
        {
            try
            {
                return new MailAddress(addr);
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {

        }
    }
}
