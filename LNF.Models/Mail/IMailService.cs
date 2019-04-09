using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public interface IMailService
    {
        string Get();
        void SendMessage(SendMessageArgs args);
    }
}
