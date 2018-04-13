using LNF.Email;
using System.Collections.Generic;

namespace LNF
{
    public interface IEmailService
    {
        string Host { get; }
        int Port { get; }
        string Username { get; }
        string Password { get; }
        bool EnableSsl { get; }
        bool Log { get; }
        string CompanyName { get; }
        bool ValidateEmail(string addr);
        void SendMessage(int clientId, string caller, string subject, string body, string from, IEnumerable<string> to = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, IEnumerable<string> attachments = null, bool isHtml = false);
        void SendMessage(SendMessageArgs args);
        IEmailGroupUtility GroupUtility { get; }
    }
}
