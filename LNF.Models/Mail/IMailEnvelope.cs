using LNF.Models.Data;
using System.Collections.Generic;
using System.Net.Mail;

namespace LNF.Models.Mail
{
    public interface IMailEnvelope
    {
        int ClientID { get; }
        string Caller { get; }
        IMailEnvelope AddRecipients(IEnumerable<string> addresses, AddressType addrType);
        IMailEnvelope AddRecipients(IEnumerable<ClientItem> addresses, AddressType addrType);
        IMailEnvelope AddRecipients(string address, AddressType addrType);
        IMailEnvelope AddRecipients(ClientItem address, AddressType addrType);
        IMailEnvelope SetFrom(string value);
        IMailEnvelope SetFrom(MailAddress value);
        IMailEnvelope SetIsBodyHtml(bool isHtml);
        void Send();
    }
}
