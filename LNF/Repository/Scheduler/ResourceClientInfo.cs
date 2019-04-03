using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository.Data;
using System;

namespace LNF.Repository.Scheduler
{
    public class ResourceClientInfo : IDataItem, IPrivileged, IAuthorized
    {
        public virtual int ResourceClientID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual ClientPrivilege Privs { get; set; }
        public virtual ClientAuthLevel AuthLevel { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual int? EmailNotify { get; set; }
        public virtual int? PracticeResEmailNotify { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Email { get; set; }
        public virtual bool ClientActive { get; set; }
        public virtual bool ResourceIsActive { get; set; }

        public virtual bool HasAuth(ClientAuthLevel auths) => (AuthLevel & auths) > 0;

        public virtual bool IsEveryone() => ClientID == -1;

        public virtual ResourceClient GetResourceClient()
        {
            var result = DA.Current.Single<ResourceClient>(ResourceClientID);

            if (result == null)
                throw new InvalidOperationException(string.Format("No ResourceClient found for ResourceClientID = {0}", ResourceClientID));

            return result;
        }

        public virtual Client GetClient()
        {
            if (IsEveryone())
                throw new InvalidOperationException(string.Format("This ResourceClient is an Everyone record. Use IsEveryone() to check for this condition before calling GetClient().", ClientID));

            var result = DA.Current.Single<Client>(ClientID);

            if (result == null)
                throw new InvalidOperationException(string.Format("No Client found for ClientID = {0}", ClientID));

            return result;
        }

        public virtual DateTime? WarningDate(double authExpWarning)
        {
            if (!Expiration.HasValue) return null;

            DateTime result = Expiration.Value.AddDays(-30 * authExpWarning * AuthDuration);

            return result;
        }
    }
}
