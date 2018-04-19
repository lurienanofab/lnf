using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace LNF.Impl.Email
{
    public class Envelope : IEmailEnvelope
    {
        private int _ClientID;
        private string _Caller;
        private MailMessage _mailMessage;
        private Message _message;

        protected IClientManager ClientManager => DA.Use<IClientManager>();

        public static IClientOrgManager ClientOrgManager => DA.Use<IClientOrgManager>();

        public int ClientID { get { return _ClientID; } }

        public string Caller { get { return _Caller; } }

        private static bool EnableLogging()
        {
            return ServiceProvider.Current.Email.Log;
        }

        private Envelope() { }

        public static Envelope Create(int clientId, string caller, MailMessage message)
        {
            Envelope result = new Envelope
            {
                _ClientID = clientId,
                _Caller = caller,
                _mailMessage = message
            };

            result.LogMessage();

            if (message.To != null && message.To.Count > 0)
                message.To.Select(x => { result.LogRecipient(x.Address, AddressType.To); return 0; }).ToList();
            if (message.CC != null & message.CC.Count > 0)
                message.CC.Select(x => { result.LogRecipient(x.Address, AddressType.Cc); return 0; }).ToList();
            if (message.Bcc != null && message.Bcc.Count > 0)
                message.Bcc.Select(x => { result.LogRecipient(x.Address, AddressType.Bcc); return 0; }).ToList();

            return result;
        }

        public static Envelope Create(int clientId, string caller, string subject, string body, string from = "", string to = "", string cc = "", string bcc = "", bool isHtml = false)
        {
            return Create(clientId, caller, subject, body, from, to.Split(','), cc.Split(','), bcc.Split(','));
        }

        public static Envelope Create(int clientId, string caller, string subject, string body, string from, IEnumerable<Client> to, IEnumerable<Client> cc, IEnumerable<Client> bcc, bool isHtml = false)
        {
            Envelope result = new Envelope();

            var primary = ClientOrgManager.GetPrimary(clientId);

            if (primary == null)
                throw new Exception(string.Format("Cannot find primary ClientOrg for ClientID {0}", clientId));

            result._mailMessage = new MailMessage
            {
                Subject = subject,
                Body = body,
                From = new MailAddress(string.IsNullOrEmpty(from) ? primary.Email : from)
            };

            result.LogMessage();

            result.AddRecipients(to, AddressType.To)
                .AddRecipients(cc, AddressType.Cc)
                .AddRecipients(bcc, AddressType.Bcc)
                .SetIsBodyHtml(isHtml);

            return result;
        }

        public static Envelope Create(int clientId, string caller, string subject, string body, string from, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc, bool isHtml = false)
        {
            Envelope result = new Envelope();

            var primary = ClientOrgManager.GetPrimary(clientId);

            if (primary == null)
                throw new Exception(string.Format("Cannot find primary ClientOrg for ClientID {0}", clientId));

            result._mailMessage = new MailMessage
            {
                Subject = subject,
                Body = body,
                From = new MailAddress(string.IsNullOrEmpty(from) ? primary.Email : from)
            };

            result.LogMessage();

            result.AddRecipients(to, AddressType.To)
                .AddRecipients(cc, AddressType.Cc)
                .AddRecipients(bcc, AddressType.Bcc)
                .SetIsBodyHtml(isHtml);

            return result;
        }

        public IEmailEnvelope AddRecipients(IEnumerable<string> addresses, AddressType addrType)
        {
            if (addresses == null) return this;
            string temp = string.Join(",", addresses);
            addresses = temp.Split(',');
            foreach (string addr in addresses)
            {
                if (!string.IsNullOrEmpty(addr))
                {
                    switch (addrType)
                    {
                        case AddressType.To:
                            _mailMessage.To.Add(addr);
                            break;
                        case AddressType.Cc:
                            _mailMessage.CC.Add(addr);
                            break;
                        case AddressType.Bcc:
                            _mailMessage.Bcc.Add(addr);
                            break;
                        default:
                            throw new ArgumentException("Invalid argument", "addrType");
                    }
                    LogRecipient(addr, addrType);
                }
            }

            return this;
        }

        public IEmailEnvelope AddRecipients(IEnumerable<Client> addresses, AddressType addrType)
        {
            if (addresses == null) return this;

            foreach (Client addr in addresses)
            {
                switch (addrType)
                {
                    case AddressType.To:
                        _mailMessage.To.Add(ClientManager.PrimaryEmail(addr));
                        break;
                    case AddressType.Cc:
                        _mailMessage.CC.Add(ClientManager.PrimaryEmail(addr));
                        break;
                    case AddressType.Bcc:
                        _mailMessage.Bcc.Add(ClientManager.PrimaryEmail(addr));
                        break;
                    default:
                        throw new ArgumentException("Invalid argument", "addrType");
                }
                LogRecipient(addr, addrType);
            }

            return this;
        }

        public IEmailEnvelope AddRecipients(string address, AddressType addrType)
        {
            return AddRecipients(new string[] { address }, addrType);
        }

        public IEmailEnvelope AddRecipients(Client address, AddressType addrType)
        {
            return AddRecipients(new Client[] { address }, addrType);
        }

        public IEmailEnvelope SetFrom(string value)
        {
            MailAddress ma = new MailAddress(value);
            SetFrom(ma);
            return this;
        }

        public IEmailEnvelope SetFrom(MailAddress value)
        {
            _mailMessage.From = value;
            if (EnableLogging())
            {
                Message msg = GetLogMessage();
                msg.FromAddress = value.Address;
            }
            return this;
        }

        public IEmailEnvelope SetIsBodyHtml(bool value)
        {
            _mailMessage.IsBodyHtml = value;
            return this;
        }

        public void Send()
        {

            SmtpClient client = new SmtpClient(ServiceProvider.Current.Email.Host, ServiceProvider.Current.Email.Port);

            if (!string.IsNullOrEmpty(ServiceProvider.Current.Email.Username) && !string.IsNullOrEmpty(ServiceProvider.Current.Email.Password))
            {
                client.Credentials = new NetworkCredential(ServiceProvider.Current.Email.Username, ServiceProvider.Current.Email.Password);
                client.EnableSsl = ServiceProvider.Current.Email.EnableSsl;
            }

            client.Send(_mailMessage);

            LogSend();
        }

        private void LogRecipient(Client recipient, AddressType addrType)
        {
            if (EnableLogging())
            {
                Message msg = GetLogMessage();

                Recipient recip = new Recipient
                {
                    Message = msg,
                    ClientID = recipient.ClientID,
                    AddressType = addrType,
                    AddressText = ClientManager.PrimaryEmail(recipient),
                    AddressTimestamp = DateTime.Now
                };

                DA.Current.Insert(recip);

                msg.Recipients.Add(recip);
            }
        }

        private void LogRecipient(string addr, AddressType addrType)
        {
            if (EnableLogging())
            {
                Message msg = GetLogMessage();

                Recipient recip = new Recipient
                {
                    Message = msg,
                    ClientID = 0,
                    AddressType = addrType,
                    AddressText = addr,
                    AddressTimestamp = DateTime.Now
                };

                DA.Current.Insert(recip);

                msg.Recipients.Add(recip);
            }
        }

        private void LogMessage()
        {
            if (EnableLogging())
            {
                Message msg = GetLogMessage();
            }
        }

        private void LogSend()
        {
            if (EnableLogging())
            {
                Message msg = GetLogMessage();
                msg.SentOn = DateTime.Now;
            }
        }

        private Message GetLogMessage()
        {
            if (_message == null)
            {
                _message = new Message
                {
                    CreatedOn = DateTime.Now,
                    ClientID = ClientID
                };

                if (string.IsNullOrEmpty(Caller))
                    throw new Exception("Caller is required.");

                _message.Caller = Caller;

                if (_mailMessage != null)
                {
                    _message.FromAddress = _mailMessage.From.Address;
                    _message.Subject = _mailMessage.Subject;
                    _message.Body = _mailMessage.Body;
                }
                DA.Current.Insert(_message);
            }
            return _message;
        }
    }
}
