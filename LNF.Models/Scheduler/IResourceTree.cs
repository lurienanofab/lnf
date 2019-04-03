using LNF.Models.Data;
using System;

namespace LNF.Models.Scheduler
{
    public interface IResourceTree : IClient, IResource, IAuthorized
    {
        int ResourceClientID { get; set; }
        ClientAuthLevel EveryoneAuthLevel { get; set; }
        ClientAuthLevel EffectiveAuthLevel { get; set; }
        DateTime? Expiration { get; set; }
        int? EmailNotify { get; set; }
        int? PracticeResEmailNotify { get; set; }
        bool HasEffectiveAuth(ClientAuthLevel auths);
    }
}
