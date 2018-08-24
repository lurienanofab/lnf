using LNF.Models.Data;
using System;

namespace LNF.Models.Scheduler
{
    public class ResourceClientItem : IPrivileged, IAuthorized
    {
        public int ResourceClientID { get; set; }
        public int ResourceID { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public ClientAuthLevel AuthLevel { get; set; }
        public DateTime? Expiration { get; set; }
        public int? EmailNotify { get; set; }
        public int? PracticeResEmailNotify { get; set; }
        public string ResourceName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool ClientActive { get; set; }

        public bool HasAuth(int auths)
        {
            return HasAuth((ClientAuthLevel)auths);
        }

        public bool HasAuth(ClientAuthLevel auths)
        {
            return (AuthLevel & auths) > 0;
        }

        public bool IsEveryone()
        {
            return ClientID == -1;
        }
    }
}
