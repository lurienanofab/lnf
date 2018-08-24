using System;

namespace LNF.Models.Scheduler
{
    public class ResourceItem : IResource
    {
        /*
        ReservFence			: stored in minutes and entered in hours (168 hours = 10080 minutes)
        Granularity			: stored in minutes and entered in minutes
        Offset				: stored in hours and entered in hours
        MinReservTime		: stored in minutes and entered in minutes
        MaxReservTime		: stored in minutes and entered in hours (4 hours = 240 minutes)
        MaxAlloc			: stored in minutes and entered in hours (20 hours = 1200 minutes)
        MinCancelTime		: stored in minutes and entered in minutes
        GracePeriod			: stored in minutes and entered in minutes
        AutoEnd				: stored in minutes and entered in minutes
        OTFSchedTime		: stored in minutes and entered in minutes
        UnloadTime			: stored in minutes and entered in minutes
        AuthorizedDuration	: stored in months and entered in months 
        */

        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public int LabID { get; set; }
        public string LabName { get; set; }
        public string LabDisplayName { get; set; }
        public int ProcessTechID { get; set; }
        public string ProcessTechName { get; set; }
        public string ResourceDescription { get; set; }
        public TimeSpan Granularity { get; set; }
        public TimeSpan ReservFence { get; set; }
        public TimeSpan MinReservTime { get; set; }
        public TimeSpan MaxReservTime { get; set; }
        public TimeSpan MaxAlloc { get; set; }
        public TimeSpan Offset { get; set; }
        public TimeSpan GracePeriod { get; set; }
        public TimeSpan AutoEnd { get; set; }
        public TimeSpan MinCancelTime { get; set; }
        public TimeSpan UnloadTime { get; set; }
        public int AuthDuration { get; set; }
        public bool AuthState { get; set; }
        public bool IsSchedulable { get; set; }
        public ResourceState State { get; set; }
        public string StateNotes { get; set; }
        public bool IsReady { get; set; }
        public bool ResourceIsActive { get; set; }
        public string HelpdeskEmail { get; set; }
        public string WikiPageUrl { get; set; }

        public bool HasState(ResourceState state)
        {
            return State == state;
        }

        public static string GetDisplayName(string resourceName, int resourceId)
        {
            return string.Format("{0} [{1}]", resourceName, resourceId);
        }

        public override string ToString()
        {
            return GetDisplayName(ResourceName, ResourceID);
        }
    }
}
