using LNF.DataAccess;
using System;

namespace LNF.Mail
{
    public class MassEmail : IMassEmail, IDataItem
    {
        public virtual int MassEmailID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual Guid EmailId { get; set; }
        public virtual string EmailFolder { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime ModifiedOn { get; set; }
        public virtual string RecipientGroup { get; set; }
        public virtual string RecipientCriteria { get; set; }
        public virtual string FromAddress { get; set; }
        public virtual string CCAddress { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }

        public virtual string[] GetCC()
        {
            if (string.IsNullOrEmpty(CCAddress))
                return new string[] { };
            else
                return CCAddress.Split(',');
        }
    }
}
