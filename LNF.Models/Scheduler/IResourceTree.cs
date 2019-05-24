using LNF.Models.Data;
using System;

namespace LNF.Models.Scheduler
{
    public interface IResourceTree : IClient, IResource, IAuthorized
    {
        int CurrentReservationID { get; set; }
        int CurrentClientID { get; set; }
        int CurrentActivityID { get; set; }
        string CurrentFirstName { get; set; }
        string CurrentLastName { get; set; }
        string CurrentActivityName { get; set; }
        bool CurrentActivityEditable { get; set; }
        DateTime? CurrentBeginDateTime { get; set; }
        DateTime? CurrentEndDateTime { get; set; }
        string CurrentNotes { get; set; }
        int ResourceClientID { get; set; }
        ClientAuthLevel EveryoneAuthLevel { get; set; }
        ClientAuthLevel EffectiveAuthLevel { get; set; }
        DateTime? Expiration { get; set; }
        int? EmailNotify { get; set; }
        int? PracticeResEmailNotify { get; set; }
        bool HasEffectiveAuth(ClientAuthLevel auths);

    }
}
