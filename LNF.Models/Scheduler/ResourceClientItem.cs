using LNF.Models.Data;
using System;

namespace LNF.Models.Scheduler
{
    public class ResourceClientItem : IResourceClient
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
        public int AuthDuration { get; set; }
        public bool ResourceIsActive { get; set; }
        public bool HasAuth(ClientAuthLevel auths) => HasAuth(AuthLevel & auths);
        public bool IsEveryone() => IsEveryone(ClientID);

        public static bool HasAuth(ClientAuthLevel auth1, ClientAuthLevel auth2) => (auth1 & auth2) > 0;

        public static bool IsEveryone(int clientId) => clientId == -1;
    }
}
