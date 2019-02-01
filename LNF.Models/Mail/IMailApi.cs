using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public interface IMailApi
    {
        string Get();
        void SendMessage(SendMessageArgs args);
    }
}
