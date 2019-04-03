using LNF.Models.Scheduler;
using System;

namespace LNF.Repository.Scheduler
{
    public class ResourceClient : IDataItem, IAuthorized
    {
        public virtual int ResourceClientID { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual int ClientID { get; set; }
        public virtual ClientAuthLevel AuthLevel { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual int? EmailNotify { get; set; }
        public virtual int? PracticeResEmailNotify { get; set; }
        public virtual bool HasAuth(ClientAuthLevel auths) => ResourceClientItem.HasAuth(AuthLevel, auths);
        public virtual bool IsEveryone() => ResourceClientItem.IsEveryone(ClientID);

        public virtual ResourceClientInfo GetResourceClientInfo()
        {
            var result = DA.Current.Single<ResourceClientInfo>(ResourceClientID);

            if (result == null)
                throw new InvalidOperationException(string.Format("No ResourceClient found for ResourceClientID = {0}", ResourceClientID));

            return result;
        }
    }
}
