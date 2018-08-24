using System;

namespace LNF.Models.Scheduler
{
    public class ResourceTreeItem : IResource
    {
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDescription { get; set; }
        public bool ResourceIsActive { get; set; }
        public int ProcessTechID { get; set; }
        public string ProcessTechName { get; set; }
        public string ProcessTechDescription { get; set; }
        public bool ProcessTechIsActive { get; set; }
        public int ProcessTechGroupID { get; set; }
        public string ProcessTechGroupName { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDisplayName { get; set; }
        public string LabDescription { get; set; }
        public bool LabIsActive { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public string BuildingDescription { get; set; }
        public bool BuildingIsActive { get; set; }
        public bool IsSchedulable { get; set; }
        public string HelpdeskEmail { get; set; }
        public string WikiPageUrl { get; set; }
        public ResourceState State { get; set; }
        public string StateNotes { get; set; }
        public int AuthDuration { get; set; }
        public bool AuthState { get; set; }
        public int ReservFence { get; set; }
        public int MaxAlloc { get; set; }
        public int MinCancelTime { get; set; }
        public int AutoEnd { get; set; }
        public int UnloadTime { get; set; }
        public int Granularity { get; set; }
        public int Offset { get; set; }
        public bool IsReady { get; set; }
        public int MinReservTime { get; set; }
        public int MaxReservTime { get; set; }
        public int GracePeriod { get; set; }
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
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public int Privs { get; set; }
        public int Communities { get; set; }
        public string DisplayName { get; set; }
        public bool ClientActive { get; set; }
        public int OrgID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int MaxChargeTypeID { get; set; }
        public int ResourceClientID { get; set; }
        public ClientAuthLevel AuthLevel { get; set; }
        public ClientAuthLevel EveryoneAuthLevel { get; set; }
        public ClientAuthLevel EffectiveAuthLevel { get; set; }
        public DateTime? Expiration { get; set; }
        public int? EmailNotify { get; set; }
        public int? PracticeResEmailNotify { get; set; }

        public ResourceItem GetResourceItem()
        {
            return new ResourceItem()
            {
                ResourceID = ResourceID,
                ResourceName = ResourceName,
                BuildingID = BuildingID,
                BuildingName = BuildingName,
                LabID = LabID,
                LabName = LabName,
                LabDisplayName = LabDisplayName,
                ProcessTechID = ProcessTechID,
                ProcessTechName = ProcessTechName,
                ResourceDescription = ResourceDescription,
                Granularity = TimeSpan.FromMinutes(Granularity),
                ReservFence = TimeSpan.FromMinutes(ReservFence),
                MinReservTime = TimeSpan.FromMinutes(MinReservTime),
                MaxReservTime = TimeSpan.FromMinutes(MaxReservTime),
                MaxAlloc = TimeSpan.FromMinutes(MaxAlloc),
                Offset = TimeSpan.FromHours(Offset),
                GracePeriod = TimeSpan.FromMinutes(GracePeriod),
                AutoEnd = TimeSpan.FromMinutes(AutoEnd),
                MinCancelTime = TimeSpan.FromMinutes(MinCancelTime),
                UnloadTime = TimeSpan.FromMinutes(UnloadTime),
                AuthDuration = AuthDuration,
                AuthState = AuthState,
                IsSchedulable = IsSchedulable,
                State = State,
                StateNotes = StateNotes,
                IsReady = IsReady,
                ResourceIsActive = ResourceIsActive,
                HelpdeskEmail = HelpdeskEmail,
                WikiPageUrl = WikiPageUrl
            };
        }
    }
}
