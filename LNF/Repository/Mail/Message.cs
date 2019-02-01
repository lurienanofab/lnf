using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF.Repository.Mail
{
    public class Message : IDataItem
    {
        public Message()
        {
            Recipients = new List<Recipient>();
        }

        public virtual int MessageID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string FromAddress { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual string Error { get; set; }
        public virtual string Caller { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? SentOn { get; set; }
        public virtual IList<Recipient> Recipients { get; set; }

        public virtual Client GetClient()
        {
            //Note: ClientID might be zero, so this result can be null.
            return DA.Current.Single<Client>(ClientID);
        }

        public virtual void AppendError(string err)
        {
            if (string.IsNullOrEmpty(err)) return;
            string nl = (string.IsNullOrEmpty(Error)) ? string.Empty : Environment.NewLine + "----------" + Environment.NewLine;
            Error += nl + err;
        }
    }
}
