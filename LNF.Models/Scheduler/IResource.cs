﻿using System;

namespace LNF.Models.Scheduler
{
    public interface IResource : IProcessTech
    {
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        bool ResourceIsActive { get; set; }
        bool IsSchedulable { get; set; }
        string ResourceDescription { get; set; }
        string HelpdeskEmail { get; set; }
        string WikiPageUrl { get; set; }
        ResourceState State { get; set; }
        string StateNotes { get; set; }
        ///// <summary>
        ///// Period that a user authorization is valid. Stored in months and entered in months 
        ///// </summary>
        int AuthDuration { get; set; }
        bool AuthState { get; set; }
        ///// <summary>
        ///// Minutes from now during which a reservation can be made. Stored in minutes and entered in hours (168 hours = 10080 minutes).
        ///// </summary>
        int ReservFence { get; set; }
        ///// <summary>
        ///// Minutes a user may reserve in total (limits the total duration of unstarted reservations within the fence). Stored in minutes and entered in hours (20 hours = 1200 minutes).
        ///// </summary>
        int MaxAlloc { get; set; }
        ///// <summary>
        ///// Minutes before the reservation start time when a user can still cancel the reservation. Stored in minutes and entered in minutes.
        ///// </summary>
        int MinCancelTime { get; set; }
        ///// <summary>
        ///// Minutes after the reservation end time when any reservation will auto-end. Zero means auto-end immediately, -1 means do not auto-end. Can be overridden by user configured per reservation auto-end. Stored in minutes and entered in minutes.
        ///// </summary>
        int ResourceAutoEnd { get; set; }
        ///// <summary>
        ///// Minutes required to unload the tool (used by interlock actions). Stored in minutes and entered in minutes.
        ///// </summary>
        int? UnloadTime { get; set; }
        ///// <summary>
        ///// Minutes that on-the-fly reservations should be scheduled for (not currently used by on-the-fly system). Stored in minutes and entered in minutes.
        ///// </summary>
        int? OTFSchedTime { get; set; }
        ///// <summary>
        ///// Minutes that are the smallest time increment a reservation may use. Stored in minutes and entered in minutes.
        ///// </summary>
        int Granularity { get; set; }
        ///// <summary>
        ///// Hours after 00:00 that specify the beginning of the day for the resource. Stored in hours and entered in hours.
        ///// </summary>
        int Offset { get; set; }
        ///// <summary>
        ///// Minutes that are the minimum allowed reservation duration. Also determines the time a user may start a reservation early. Stored in minutes and entered in minutes.
        ///// </summary>
        int MinReservTime { get; set; }
        ///// <summary>
        ///// Minutes that are the maximum allowed reservation duration. Stored in minutes and entered in hours (4 hours = 240 minutes).
        ///// </summary>
        int MaxReservTime { get; set; }
        ///// <summary>
        ///// Minutes after the reservation start time during which a user may start a reservation. Stored in minutes and entered in minutes.
        ///// </summary>
        int GracePeriod { get; set; }
        bool IsReady { get; set; }
        int CurrentReservationID { get; set; }
        int CurrentClientID { get; set; }
        int CurrentActivityID { get; set; }
        bool CurrentActivityEditable { get; set; }
        string CurrentFirstName { get; set; }
        string CurrentLastName { get; set; }
        string CurrentActivityName { get; set; }
        DateTime? CurrentBeginDateTime { get; set; }
        DateTime? CurrentEndDateTime { get; set; }
        string CurrentNotes { get; set; }
        string ResourceDisplayName { get; }
        bool HasState(ResourceState state);
    }
}
