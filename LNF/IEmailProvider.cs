using LNF.Email;
using System.Collections.Generic;

namespace LNF
{
    public interface IEmailProvider : ITypeProvider
    {
        string Host { get; set; }
        int Port { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool EnableSsl { get; set; }
        bool Log { get; set; }
        string CompanyName { get; }
        bool ValidateEmail(string addr);
        SendMessageResult SendMessage(int clientId, string caller, string subject, string body, string from, IEnumerable<string> to = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, IEnumerable<string> attachments = null, bool isHtml = false);
        SendMessageResult SendMessage(SendMessageArgs args);
        IEmailGroupUtility GroupUtility { get; }
    }
}
