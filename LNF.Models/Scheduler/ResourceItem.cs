﻿using System;

namespace LNF.Models.Scheduler
{
    public class ResourceItem : ProcessTechItem, IResource
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
        public override string ToString() => GetResourceDisplayName(ResourceName, ResourceID);

        public string ResourceDisplayName => GetResourceDisplayName(ResourceName, ResourceID);

        public bool HasState(ResourceState state) => HasState(State, state);

        public static bool HasState(ResourceState s1, ResourceState s2) => s1 == s2;

        public static string GetResourceDisplayName(string resourceName, int resourceId) => string.Format("{0} [{1}]", resourceName, resourceId);
    }
}
