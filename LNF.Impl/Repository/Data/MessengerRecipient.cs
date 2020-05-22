using LNF.Data;
using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class MessengerRecipient : IMessengerRecipient, IDataItem
    {
        public virtual int RecipientID { get; set; }
        public virtual int MessageID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string Folder { get; set; }
        public virtual DateTime Received { get; set; }
        public virtual DateTime? Acknowledged { get; set; }
        public virtual int AccessCount { get; set; }
    }
}
