using LNF.Mail;
using LNF.Mail.Criteria;
using LNF.Repository.Data;
using System;

namespace LNF.Repository.Mail
{
    public class MassEmail : IDataItem
    {
        public virtual int MassEmailID { get; set; }
        public virtual Client Client { get; set; }
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

        public virtual IRecipientCriteria GetCriteria()
        {
            switch (RecipientGroup)
            {
                case "community":
                    return GetCriteria<ByCommunity>();
                case "manager":
                    return GetCriteria<ByManager>();
                case "tool":
                    return GetCriteria<ByTool>();
                case "lab":
                    return GetCriteria<ByLab>();
                default: //privilege
                    return GetCriteria<ByPrivilege>();
            }
        }

        public virtual T GetCriteria<T>() where T : IRecipientCriteria, new()
        {
            if (string.IsNullOrEmpty(RecipientCriteria))
                return new T();
            else
                return ServiceProvider.Current.Serialization.Json.Deserialize<T>(RecipientCriteria);
        }

        public virtual string[] GetCC()
        {
            if (string.IsNullOrEmpty(CCAddress))
                return new string[] { };
            else
                return CCAddress.Split(',');
        }
    }
}
