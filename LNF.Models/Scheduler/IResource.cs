using System;

namespace LNF.Models.Scheduler
{
    public interface IResource
    {
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        int BuildingID { get; set; }
        string BuildingName { get; set; }
        int LabID { get; set; }
        string LabName { get; set; }
        string LabDisplayName { get; set; }
        int ProcessTechID { get; set; }
        string ProcessTechName { get; set; }
        string ResourceDescription { get; set; }

        /// <summary>
        /// Minutes that are the smallest time increment a reservation may use. Stored in minutes and entered in minutes.
        /// </summary>
        int Granularity { get; set; }

        /// <summary>
        /// Minutes from now during which a reservation can be made. Stored in minutes and entered in hours (168 hours = 10080 minutes).
        /// </summary>
        int ReservFence { get; set; }

        /// <summary>
        /// Minutes that are the minimum allowed reservation duration. Also determines the time a user may start a reservation early. Stored in minutes and entered in minutes.
        /// </summary>
        int MinReservTime { get; set; }

        /// <summary>
        /// Minutes that are the maximum allowed reservation duration. Stored in minutes and entered in hours (4 hours = 240 minutes).
        /// </summary>
        int MaxReservTime { get; set; }

        /// <summary>
        /// Minutes a user may reserve in total (limits the total duration of unstarted reservations within the fence). Stored in minutes and entered in hours (20 hours = 1200 minutes).
        /// </summary>
        int MaxAlloc { get; set; }

        /// <summary>
        /// Hours after 00:00 that specify the beginning of the day for the resource. Stored in hours and entered in hours.
        /// </summary>
        int Offset { get; set; }

        /// <summary>
        /// Minutes after the reservation start time during which a user may start a reservation. Stored in minutes and entered in minutes.
        /// </summary>
        int GracePeriod { get; set; }

        /// <summary>
        /// Minutes after the reservation end time when any reservation will auto-end. Zero means auto-end immediately, -1 means do not auto-end. Can be overridden by user configured per reservation auto-end. Stored in minutes and entered in minutes.
        /// </summary>
        int ResourceAutoEnd { get; set; }

        /// <summary>
        /// Minutes before the reservation start time when a user can still cancel the reservation. Stored in minutes and entered in minutes.
        /// </summary>
        int MinCancelTime { get; set; }

        /// <summary>
        /// Minutes required to unload the tool (used by interlock actions). Stored in minutes and entered in minutes.
        /// </summary>
        int UnloadTime { get; set; }

        /// <summary>
        /// Minutes that on-the-fly reservations are scheduled (not used used by current OTF system). Stored in minutes and entered in minutes.
        /// </summary>
        int OTFSchedTime { get; set; }

        bool AuthState { get; set; }

        /// <summary>
        /// Period that a user authorization is valid. Stored in months and entered in months 
        /// </summary>
        int AuthDuration { get; set; }

        string StateNotes { get; set; }
        bool IsSchedulable { get; set; }
        bool ResourceIsActive { get; set; }
        string HelpdeskEmail { get; set; }
        string WikiPageUrl { get; set; }
        bool HasState(ResourceState state);
    }
}
