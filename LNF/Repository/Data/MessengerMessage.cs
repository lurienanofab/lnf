using System;

namespace LNF.Repository.Data
{
    public class MessengerMessage : IDataItem
    {
        public virtual int MessageID { get; set; }
        public virtual int ParentID { get; set; }
        public virtual Client Client { get; set; }
        public virtual string Status { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime? Sent { get; set; }
        public virtual bool DisableReply { get; set; }
        public virtual bool Exclusive { get; set; }
        public virtual bool AcknowledgeRequired { get; set; }
        public virtual bool BlockAccess { get; set; }
        public virtual int AccessCutoff { get; set; }
    }
}
