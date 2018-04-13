using LNF.Repository.Data;
using LNF.Repository.Email;
using System.Collections.Generic;
using System.Net.Mail;

namespace LNF
{
    public interface IEmailEnvelope
    {
        int ClientID { get; }
        string Caller { get; }
        IEmailEnvelope AddRecipients(IEnumerable<string> addresses, AddressType addrType);
        IEmailEnvelope AddRecipients(IEnumerable<Client> addresses, AddressType addrType);
        IEmailEnvelope AddRecipients(string address, AddressType addrType);
        IEmailEnvelope AddRecipients(Client address, AddressType addrType);
        IEmailEnvelope SetFrom(string value);
        IEmailEnvelope SetFrom(MailAddress value);
        IEmailEnvelope SetIsBodyHtml(bool isHtml);
        void Send();
    }
}
