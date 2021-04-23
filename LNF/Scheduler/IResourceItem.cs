using System;

namespace LNF.Scheduler
{
    public interface IResourceItem : IProcessTechItem
    {
        int ResourceID { get; set; }

        string ResourceName { get; set; }

        string ResourceDisplayName { get; }

        /// <summary>
        /// Minutes from now during which a reservation can be made. Stored in minutes and entered in hours (168 hours = 10080 minutes).
        /// </summary>
        int ReservFence { get; set; }

        /// <summary>
        /// Minutes that are the smallest time increment a reservation may use. Stored in minutes and entered in minutes.
        /// </summary>
        int Granularity { get; set; }

        /// <summary>
        /// Hours after 00:00 that specify the beginning of the day for the resource. Stored in hours and entered in hours.
        /// </summary>
        int Offset { get; set; }

        /// <summary>
        /// Minutes that are the minimum allowed reservation duration. Also determines the time a user may start a reservation early. Stored in minutes and entered in minutes.
        /// </summary>
        int MinReservTime { get; set; }

        /// <summary>
        /// Minutes that are the maximum allowed reservation duration. Stored in minutes and entered in hours (4 hours = 240 minutes).
        /// </summary>
        int MaxReservTime { get; set; }

        /// <summary>
        /// Minutes before the reservation start time when a user can still cancel the reservation. Stored in minutes and entered in minutes.
        /// </summary>
        int MinCancelTime { get; set; }

        /// <summary>
        /// Minutes after the reservation start time during which a user may start a reservation. Stored in minutes and entered in minutes.
        /// </summary>
        int GracePeriod { get; set; }

        /// <summary>
        /// Minutes after the reservation end time when any reservation will auto-end. Zero means auto-end immediately, -1 means do not auto-end. Can be overridden by user configured per reservation auto-end. Stored in minutes and entered in minutes.
        /// </summary>
        int ResourceAutoEnd { get; set; }

        /// <summary>
        /// Period that a user authorization is valid. Stored in months and entered in months 
        /// </summary>
        int AuthDuration { get; set; }

        bool AuthState { get; set; }

        /// <summary>
        /// Returns the next granularity boundary in the past or future.
        /// </summary>
        /// <param name="now">The point in time to determine the next or previous granularity</param>
        /// <param name="dir">The direction (next or pervious) to search in</param>
        /// <returns>The DateTime value of the next or previous granularity</returns>
        DateTime GetNextGranularity(DateTime now, GranularityDirection dir);
    }
}
