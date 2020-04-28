using System;

namespace LNF.Mail
{
    public interface IMessage
    {
        int MessageID { get; set; }
        int ClientID { get; set; }
        string FromAddress { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        string Error { get; set; }
        string Caller { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime? SentOn { get; set; }
    }
}