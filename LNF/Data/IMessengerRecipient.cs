using System;

namespace LNF.Data
{
    public interface IMessengerRecipient
    {
        int RecipientID { get; set; }
        int MessageID { get; set; }
        int ClientID { get; set; }
        string Folder { get; set; }
        DateTime Received { get; set; }
        DateTime? Acknowledged { get; set; }
        int AccessCount { get; set; }
    }
}
