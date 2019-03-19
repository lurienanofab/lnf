using LNF.Mail;
using LNF.Mail.Criteria;
using LNF.Repository.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public virtual bool Send(out string message, ResourceTreeItemCollection resourceTree, int clientId)
        {
            bool result = true;

            IEnumerable<MassEmailRecipient> recipients = GetCriteria().GetRecipients();

            int recipientCount = recipients.Count();

            int ccCount = 0;
            string[] cc = null;

            if (!string.IsNullOrEmpty(CCAddress))
            {
                // we know all cc addresses are valid because this is checked in IsValid
                cc = CCAddress.Split(',');
                ccCount = cc.Length;
            }

            if (recipientCount + ccCount > 0)
            {
                var mgr = new GroupEmailManager(resourceTree);
                var sendResult = mgr.SendEmail(this, clientId);

                if (string.IsNullOrEmpty(sendResult))
                {
                    message = string.Format("Message sent to {0} recipient{1}{2}.",
                        recipientCount,
                        recipientCount != 1 ? "s" : string.Empty,
                        ccCount > 0 ? string.Format(" and {0} cc address{1}", ccCount, ccCount != 1 ? "es" : string.Empty) : string.Empty);

                    ModifiedOn = DateTime.Now;
                    EmailFolder = "sent";
                }
                else
                {
                    message = sendResult;
                    result = false;
                }
            }
            else
            {
                message = "No recipients were found, message not sent.";
                result = false;
            }

            return result;
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
