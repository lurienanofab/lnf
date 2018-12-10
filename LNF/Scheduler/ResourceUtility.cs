using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;

namespace LNF.Scheduler
{
    public enum GranularityDirection
    {
        Previous = 0,
        Next = 1
    }

    public static class ResourceUtility
    {
        public static void UpdateState(int resourceId, ResourceState state, string stateNotes)
        {
            // procResourceUpdate @Action = 'UpdateState'

            //UPDATE dbo.Resource
            //SET State = @State,
            //StateNotes = @StateNotes
            //WHERE ResourceID = @ResourceID

            var res = DA.Current.Single<Resource>(resourceId);

            if (res != null)
            {
                res.State = state;
                res.StateNotes = stateNotes;
            }
        }

        public static void EngineerUpdate(ResourceItem model)
        {
            // procResourceUpdate @Action = 'EngineerUpdate'

            //UPDATE dbo.Resource
            //SET ResourceName = @ResourceName,
            //  UseCost = @UseCost,
            //  HourlyCost = @HourlyCost,
            //  AuthDuration = @AuthDuration,
            //  AuthState = @AuthState,
            //  ReservFence = @ReservFence * 60,
            //  Granularity = @Granularity,
            //  Offset = @Offset,
            //  MinReservTime = @MinReservTime,
            //  MaxReservTime = @MaxReservTime * 60,
            //  MaxAlloc = @MaxAlloc * 60,
            //  MinCancelTime = @MinCancelTime,
            //  GracePeriod = @GracePeriod,
            //  AutoEnd = @AutoEnd,
            //  OTFSchedTime = @OTFSchedTime,
            //  IPAddress = @IPAddress,
            //  Description = @Description,
            //  IsReady = 1,
            //  UnloadTime = @UnloadTime
            //WHERE ResourceID = @ResourceID

            // need to check ReservFence, MaxReservTime, and MaxAlloc

            var res = DA.Current.Single<Resource>(model.ResourceID);

            if (res != null)
            {
                res.ResourceName = model.ResourceName;
                res.UseCost = 0;
                res.HourlyCost = 0;
                res.AuthDuration = model.AuthDuration;
                res.AuthState = model.AuthState;
                res.ReservFence = Convert.ToInt32(model.ReservFence.TotalMinutes);
                res.Granularity = Convert.ToInt32(model.Granularity.TotalMinutes);
                res.Offset = Convert.ToInt32(model.Offset.TotalHours);
                res.MinReservTime = Convert.ToInt32(model.MinReservTime.TotalMinutes);
                res.MaxReservTime = Convert.ToInt32(model.MaxReservTime.TotalMinutes);
                res.MaxAlloc = Convert.ToInt32(model.MaxAlloc.TotalMinutes);
                res.MinCancelTime = Convert.ToInt32(model.MinCancelTime.TotalMinutes);
                res.GracePeriod = Convert.ToInt32(model.GracePeriod.TotalMinutes);
                res.AutoEnd = Convert.ToInt32(model.AutoEnd.TotalMinutes);
                res.OTFSchedTime = null;
                res.IPAddress = null;
                res.Description = model.ResourceDescription;
                res.WikiPageUrl = model.WikiPageUrl;
                res.IsReady = true;
                res.UnloadTime = Utility.GetNullableMinutesFromTimeSpan(model.UnloadTime);
            }
        }

        /// <summary>
        /// Returns the next grain boundary in the past or future
        /// </summary>
        /// <param name="granularity">The time increment in minutes a resource can be reserved</param>
        /// <param name="offset">The offset hours that specify the beginning of the day for a resource</param>
        /// <param name="actualTime">The point in time to determine the next or previous granularity</param>
        /// <param name="granDir">The direction (next or pervious) to search in</param>
        /// <returns>The DateTime value of the next or previous granularity</returns>
        public static DateTime GetNextGranularity(TimeSpan granularity, TimeSpan offset, DateTime actualTime, GranularityDirection granDir)
        {
            // get number of minutes between now and beginning of day (midnight + offset) of passed-in date
            DateTime dayBegin = new DateTime(actualTime.Year, actualTime.Month, actualTime.Day).Add(offset);

            double repairBeginMinutes = actualTime.Subtract(dayBegin).TotalMinutes;

            if (repairBeginMinutes % granularity.TotalMinutes == 0)
                return actualTime; //this is a granularity boundary
            else
            {
                int numOfGrans = Convert.ToInt32(repairBeginMinutes / granularity.TotalMinutes);
                return dayBegin.AddMinutes((numOfGrans + (int)granDir) * granularity.TotalMinutes);
            }
        }

        /// <summary>
        /// Sets the start and end time slot boundaries
        /// </summary>
        /// <param name="granularity">The time increment in minutes a resource can be reserved</param>
        /// <param name="offset">The offset hours that specify the beginning of the day for a resource</param>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        public static void GetTimeSlotBoundary(TimeSpan granularity, TimeSpan offset, ref DateTime startTime, ref DateTime endTime)
        {
            double resHours = 0;
            if (granularity.TotalMinutes > 60) resHours = granularity.TotalMinutes / 60D;

            double maxEndHour = 23;
            if (offset.TotalHours > 0) maxEndHour = offset.TotalHours + 24 - resHours;

            if (CacheManager.Current.DisplayDefaultHours())
            {
                double defaultBeginHour = CacheManager.Current.GetClientSetting().GetBeginHourOrDefault();
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

                double defaultEndHour = CacheManager.Current.GetClientSetting().GetEndHourOrDefault();
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
    }
}
