using System;

namespace LNF.Data
{
    public interface IMessengerMessage
    {
        int MessageID { get; set; }
        int ParentID { get; set; }
        int ClientID { get; set; }
        string Status { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        DateTime Created { get; set; }
        DateTime? Sent { get; set; }
        bool DisableReply { get; set; }
        bool Exclusive { get; set; }
        bool AcknowledgeRequired { get; set; }
        bool BlockAccess { get; set; }
        int AccessCutoff { get; set; }
    }
}
