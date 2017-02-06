using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Email;

namespace LNF
{
    public interface IEmailEnvelope
    {
        int ClientID { get; }
        string Caller { get; }
        Exception Exception { get; }
        Message GetLogMessage();
        IEmailEnvelope AddRecipients(IEnumerable<string> addresses, AddressType addrType);
        IEmailEnvelope AddRecipients(IEnumerable<Client> addresses, AddressType addrType);
        IEmailEnvelope AddRecipients(string address, AddressType addrType);
        IEmailEnvelope AddRecipients(Client address, AddressType addrType);
        IEmailEnvelope SetFrom(string value);
        IEmailEnvelope SetFrom(MailAddress value);
        IEmailEnvelope SetIsBodyHtml(bool isHtml);
        bool Send();
    }
}
