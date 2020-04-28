using LNF.CommonTools;
using System;

namespace LNF.Scheduler
{
    public static class Resources
    {
        public static void UpdateState(int resourceId, ResourceState state, string stateNotes)
        {
            ServiceProvider.Current.Scheduler.Resource.UpdateResourceState(resourceId, state, stateNotes);
        }

        public static void EngineerUpdate(IResource res)
        {
            ServiceProvider.Current.Scheduler.Resource.UpdateResource(
                resourceId: res.ResourceID,
                resourceName: res.ResourceName,
                useCost: 0,
                hourlyCost: 0,
                authDuration: res.AuthDuration,
                authState: res.AuthState,
                reservFence: res.ReservFence,
                granularity: res.Granularity,
                offset: res.Offset,
                minReservTime: res.MinReservTime,
                maxReservTime: res.MaxReservTime,
                maxAlloc: res.MaxAlloc,
                minCancelTime: res.MinCancelTime,
                gracePeriod: res.GracePeriod,
                autoEnd: res.ResourceAutoEnd,
                otfSchedTime: null,
                ipAddress: null,
                description: res.ResourceDescription,
                wikiPageUrl: res.WikiPageUrl,
                isReady: true,
                unloadTime: Utility.GetNullableMinutesFromTimeSpan(TimeSpan.FromMinutes(res.UnloadTime.GetValueOrDefault()))
            );
        }

        /// <summary>
        /// Sets the start and end time slot boundaries
        /// </summary>
        /// <param name="granularity">The time increment in minutes a resource can be reserved</param>
        /// <param name="offset">The offset hours that specify the beginning of the day for a resource</param>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        public static void GetTimeSlotBoundary(TimeSpan granularity, TimeSpan offset, ref DateTime startTime, ref DateTime endTime, bool displayDefaultHours, double defaultBeginHour, double defaultEndHour)
        {
            double resHours = 0;
            if (granularity.TotalMinutes > 60) resHours = granularity.TotalMinutes / 60D;

            double maxEndHour = 23;
            if (offset.TotalHours > 0) maxEndHour = offset.TotalHours + 24 - resHours;

            if (displayDefaultHours)
            {
                if (defaultBeginHour < offset.TotalHours) //start time cannot be earlier than the beginning of the day
                    startTime = startTime.Add(offset);
                else if (granularity.TotalMinutes > 60) //align to previous boundary
                {
                    double hours = (defaultBeginHour - offset.TotalHours) % resHours;
                    if (hours == 0)
                        startTime = startTime.AddHours(defaultBeginHour);
                    else
                        startTime = startTime.AddHours(defaultBeginHour - hours);
                }
                else
                    startTime = startTime.AddHours(defaultBeginHour);

                if (defaultEndHour > maxEndHour) //start time cannot be earlier than the beginning of the day
                    endTime = endTime.AddHours(maxEndHour);
                else if (granularity.TotalMinutes > 60) //align to next boundary
                {
                    double hours = (defaultEndHour - offset.TotalHours + resHours) % resHours;
                    if (hours == 0)
                        endTime = endTime.AddHours(defaultEndHour);
                    else
                        endTime = endTime.AddHours(defaultEndHour + hours);
                }
                else
                    endTime = endTime.AddHours(defaultEndHour);
            }
            else
            {
                startTime = startTime.Add(offset);
                endTime = endTime.AddHours(maxEndHour);
            }

            // 2007-08-09 This code is added because originally it only runs up to 11:00pm.  So we add 59 more minutes so the time slots after 11:00pm can be shown
            // [2016-12-15 jg] Now adding 1 hour to get to 00:00:00. Be sure to check for < after this, not <=
            endTime = endTime.AddHours(1);
        }

        public static string GetResourceDisplayName(string resourceName, int resourceId) => $"{resourceName} [{resourceId}]";

        public static DateTime GetNextGranularity(int gran, int offset, DateTime now, GranularityDirection dir)
        {
            // get number of minutes between now and beginning of day (midnight + offset) of passed-in date
            DateTime dayBegin = now.Date.AddHours(offset);

            double repairBeginMinutes = now.Subtract(dayBegin).TotalMinutes;

            if (repairBeginMinutes % gran == 0)
                return now; //this is a granularity boundary
            else
            {
                int numGrans = Convert.ToInt32(repairBeginMinutes / gran);
                return dayBegin.AddMinutes((numGrans + (int)dir) * gran);
            }
        }

        public static string GetResourceName(IResource res, ResourceNamePartial part)
        {
            switch (part)
            {
                case ResourceNamePartial.ResourceName:
                    return res.ResourceName;
                case ResourceNamePartial.ProcessTechName:
                    return res.ProcessTechName + ": " + res.ResourceName;
                case ResourceNamePartial.LabName:
                    return res.LabDisplayName + ": " + res.ProcessTechName + ": " + res.ResourceName;
                case ResourceNamePartial.BuildingName:
                    return res.BuildingName + ": " + res.LabDisplayName + ": " + res.ProcessTechName + ": " + res.ResourceName;
                default:
                    throw new NotSupportedException($"Unknown ResourceNamePartial value: {part}");
            }
        }

        public static bool HasState(ResourceState s1, ResourceState s2) => s1 == s2;
    }
}
