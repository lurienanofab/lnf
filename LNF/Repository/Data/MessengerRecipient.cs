using System;

namespace LNF.Repository.Data
{
    public class MessengerRecipient : IDataItem
    {
        public virtual int RecipientID { get; set; }
        public virtual MessengerMessage Message { get; set; }
        public virtual Client Client { get; set; }
        public virtual string Folder { get; set; }
        public virtual DateTime Received { get; set; }
        public virtual DateTime? Acknowledged { get; set; }
        public virtual int AccessCount { get; set; }
    }
}
