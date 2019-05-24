using System;

namespace LNF.Models.Mail
{
    public interface IMessage
    {
        string Body { get; set; }
        string Caller { get; set; }
        int ClientID { get; set; }
        DateTime CreatedOn { get; set; }
        string Error { get; set; }
        string FromAddress { get; set; }
        int MessageID { get; set; }
        DateTime? SentOn { get; set; }
        string Subject { get; set; }
    }
}