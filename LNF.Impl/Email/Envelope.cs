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
        private Exception _Exception;

        private MailMessage _mailMessage;
        private Message _message;

        public int ClientID { get { return _ClientID; } }

        public string Caller { get { return _Caller; } }

        public Exception Exception { get { return _Exception; } }

        public bool EnableLogging()
        {
            return Providers.Email.Log;
        }

        private Envelope() { }

        public static Envelope Create(int clientId, string caller, MailMessage message)
        {
            Envelope result = new Envelope();
            try
            {
                result._ClientID = clientId;
                result._Caller = caller;
                result._mailMessage = message;
                result.LogMessage();
                if (message.To != null && message.To.Count > 0)
                    message.To.Select(x => { result.LogRecipient(x.Address, AddressType.To); return 0; }).ToList();
                if (message.CC != null & message.CC.Count > 0)
                    message.CC.Select(x => { result.LogRecipient(x.Address, AddressType.Cc); return 0; }).ToList();
                if (message.Bcc != null && message.Bcc.Count > 0)
                    message.Bcc.Select(x => { result.LogRecipient(x.Address, AddressType.Bcc); return 0; }).ToList();
            }
            catch (Exception ex)
            {
                result.HandleException(ex);
            }
            return result;
        }

        public static Envelope Create(int clientId, string caller, string subject, string body, string from = "", string to = "", string cc = "", string bcc = "", bool isHtml = false)
        {
            return Create(clientId, caller, subject, body, from, to.Split(','), cc.Split(','), bcc.Split(','));
        }

        public static Envelope Create(int clientId, string caller, string subject, string body, string from, IEnumerable<Client> to, IEnumerable<Client> cc, IEnumerable<Client> bcc, bool isHtml = false)
        {
            Envelope result = new Envelope();
            try
            {
                var primary = ClientOrgUtility.GetPrimary(clientId);

                if (primary == null)
                    throw new Exception(string.Format("Cannot find primary ClientOrg for ClientID {0}", clientId));

                result._mailMessage = new MailMessage();
                result._mailMessage.Subject = subject;
                result._mailMessage.Body = body;
                result._mailMessage.From = new MailAddress(string.IsNullOrEmpty(from) ? primary.Email : from);
                result.LogMessage();
                result.AddRecipients(to, AddressType.To)
                    .AddRecipients(cc, AddressType.Cc)
                    .AddRecipients(bcc, AddressType.Bcc)
                    .SetIsBodyHtml(isHtml);
            }
            catch (Exception ex)
            {
                result.HandleException(ex);
            }
            return result;
        }

        public static Envelope Create(int clientId, string caller, string subject, string body, string from, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc, bool isHtml = false)
        {
            Envelope result = new Envelope();
            try
            {
                var primary = ClientOrgUtility.GetPrimary(clientId);

                if (primary == null)
                    throw new Exception(string.Format("Cannot find primary ClientOrg for ClientID {0}", clientId));

                result._mailMessage = new MailMessage();
                result._mailMessage.Subject = subject;
                result._mailMessage.Body = body;
                result._mailMessage.From = new MailAddress(string.IsNullOrEmpty(from) ? primary.Email : from);
                result.LogMessage();
                result.AddRecipients(to, AddressType.To)
                    .AddRecipients(cc, AddressType.Cc)
                    .AddRecipients(bcc, AddressType.Bcc)
                    .SetIsBodyHtml(isHtml);
            }
            catch (Exception ex)
            {
                result.HandleException(ex);
            }
            return result;
        }

        public IEmailEnvelope AddRecipients(IEnumerable<string> addresses, AddressType addrType)
        {
            if (addresses == null) return this;
            string temp = string.Join(",", addresses);
            addresses = temp.Split(',');
            foreach (string addr in addresses)
            {
                try
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
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            return this;
        }

        public IEmailEnvelope AddRecipients(IEnumerable<Client> addresses, AddressType addrType)
        {
            if (addresses == null) return this;
            foreach (Client addr in addresses)
            {
                try
                {
                    switch (addrType)
                    {
                        case AddressType.To:
                            _mailMessage.To.Add(addr.PrimaryEmail());
                            break;
                        case AddressType.Cc:
                            _mailMessage.CC.Add(addr.PrimaryEmail());
                            break;
                        case AddressType.Bcc:
                            _mailMessage.Bcc.Add(addr.PrimaryEmail());
                            break;
                        default:
                            throw new ArgumentException("Invalid argument", "addrType");
                    }
                    LogRecipient(addr, addrType);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
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
            try
            {
                MailAddress ma = new MailAddress(value);
                SetFrom(ma);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
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

        public bool Send()
        {
            bool result = false;
            try
            {
                SmtpClient client = new SmtpClient(Providers.Email.Host, Providers.Email.Port);
                if (!string.IsNullOrEmpty(Providers.Email.Username) && !string.IsNullOrEmpty(Providers.Email.Password))
                { 
                    client.Credentials = new NetworkCredential(Providers.Email.Username, Providers.Email.Password);
                    client.EnableSsl = Providers.Email.EnableSsl;
                }
                client.Send(_mailMessage);
                LogSend();
                result = true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = false;
            }
            return result;
        }

        private void HandleException(Exception ex)
        {
            _Exception = ex;
            LogError(ex);
        }

        private void LogRecipient(Client recipient, AddressType addrType)
        {
            if (EnableLogging())
            {
                Message msg = GetLogMessage();

                Recipient recip = new Recipient();
                recip.Message = msg;
                recip.ClientID = recipient.ClientID;
                recip.AddressType = addrType;
                recip.AddressText = recipient.PrimaryEmail();
                recip.AddressTimestamp = DateTime.Now;
                DA.Current.Insert(recip);

                msg.Recipients.Add(recip);
            }
        }

        private void LogRecipient(string addr, AddressType addrType)
        {
            if (EnableLogging())
            {
                Message msg = GetLogMessage();

                Recipient recip = new Recipient();
                recip.Message = msg;
                recip.ClientID = 0;
                recip.AddressType = addrType;
                recip.AddressText = addr;
                recip.AddressTimestamp = DateTime.Now;
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

        private void LogError(Exception ex)
        {
            if (EnableLogging())
            {
                Message msg = GetLogMessage();
                msg.AppendError(ex.Message);
            }
        }

        public Message GetLogMessage()
        {
            if (_message == null)
            {
                _message = new Message();
                _message.CreatedOn = DateTime.Now;
                _message.ClientID = ClientID;
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
