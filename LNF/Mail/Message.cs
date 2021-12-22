using LNF.DataAccess;
using System;

namespace LNF.Mail
{
    public class Message : IDataItem
    {
        public virtual int MessageID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string FromAddress { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual string Error { get; set; }
        public virtual string Caller { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? SentOn { get; set; }

        public virtual void AppendError(string err)
        {
            if (string.IsNullOrEmpty(err)) return;
            string nl = (string.IsNullOrEmpty(Error)) ? string.Empty : Environment.NewLine + "----------" + Environment.NewLine;
            Error += nl + err;
        }
    }
}
