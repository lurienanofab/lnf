using LNF.Models.Data;
using System;

namespace LNF.Models.Scheduler
{
    public class ResourceTreeItem : IResourceTree
    {
        public int CurrentReservationID { get; set; }
        public int CurrentClientID { get; set; }
        public int CurrentActivityID { get; set; }
        public string CurrentFirstName { get; set; }
        public string CurrentLastName { get; set; }
        public string CurrentActivityName { get; set; }
        public bool CurrentActivityEditable { get; set; }
        public DateTime? CurrentBeginDateTime { get; set; }
        public DateTime? CurrentEndDateTime { get; set; }
        public string CurrentNotes { get; set; }
        public int ResourceClientID { get; set; }
        public ClientAuthLevel EveryoneAuthLevel { get; set; }
        public ClientAuthLevel EffectiveAuthLevel { get; set; }
        public DateTime? Expiration { get; set; }
        public int? EmailNotify { get; set; }
        public int? PracticeResEmailNotify { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public bool ResourceIsActive { get; set; }
        public bool IsSchedulable { get; set; }
        public string ResourceDescription { get; set; }
        public string HelpdeskEmail { get; set; }
        public string WikiPageUrl { get; set; }
        public ResourceState State { get; set; }
        public string StateNotes { get; set; }
        public int AuthDuration { get; set; }
        public bool AuthState { get; set; }
        public int ReservFence { get; set; }
        public int MaxAlloc { get; set; }
        public int MinCancelTime { get; set; }
        public int ResourceAutoEnd { get; set; }
        public int? UnloadTime { get; set; }
        public int? OTFSchedTime { get; set; }
        public int Granularity { get; set; }
        public int Offset { get; set; }
        public bool IsReady { get; set; }
        public int MinReservTime { get; set; }
        public int MaxReservTime { get; set; }
        public int GracePeriod { get; set; }
        public string ResourceDisplayName => ResourceItem.GetResourceDisplayName(ResourceName, ResourceID);
        public int ProcessTechID { get; set; }
        public int ProcessTechGroupID { get; set; }
        public string ProcessTechGroupName { get; set; }
        public string ProcessTechDescription { get; set; }
        public bool ProcessTechIsActive { get; set; }
        public string ProcessTechName { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDescription { get; set; }
        public string LabDisplayName { get; set; }
        public bool LabIsActive { get; set; }
        public string BuildingDescription { get; set; }
        public int BuildingID { get; set; }
        public bool BuildingIsActive { get; set; }
        public string BuildingName { get; set; }
        public int ClientID { get; set; }
        public ClientAuthLevel AuthLevel { get; set; }
        public string UserName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public int ResourceClientClientID { get; set; }

        public DateTime GetNextGranularity(DateTime now, GranularityDirection dir) => ResourceItem.GetNextGranularity(Granularity, Offset, now, dir);

        public string GetResourceName(ResourceNamePartial part) => ResourceItem.GetResourceName(this, part);

        public bool HasAuth(ClientAuthLevel auths) => ResourceClientItem.HasAuth(AuthLevel, auths);

        public bool HasEffectiveAuth(ClientAuthLevel auths) => ResourceClientItem.HasAuth(EffectiveAuthLevel, auths);

        public bool HasState(ResourceState state) => ResourceItem.HasState(State, state);

        public bool IsEveryone() => ResourceClientItem.IsEveryone(ResourceClientClientID);
    }
}
