using LNF.Data;
using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ResourceClientInfo : IResourceClient, IDataItem
    {
        public virtual int ResourceClientID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int AuthDuration { get; set; }
        public virtual bool ResourceIsActive { get; set; }
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual bool ClientActive { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual int? EmailNotify { get; set; }
        public virtual int? PracticeResEmailNotify { get; set; }
        public virtual ClientAuthLevel AuthLevel { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string LName { get; set; }
        public virtual string MName { get; set; }
        public virtual string FName { get; set; }
        public virtual ClientPrivilege Privs { get; set; }
        public virtual bool HasAuth(ClientAuthLevel auths) => ResourceClients.HasAuth(AuthLevel, auths);
        public virtual bool IsEveryone() => ResourceClients.IsEveryone(ClientID);
        public virtual DateTime? WarningDate(double authExpWarning) => ResourceClients.GetWarningDate(this);

        public override string ToString()
        {
            return $"{DisplayName} [{ClientID}/{UserName}]: {AuthLevel}";
        }
    }
}
