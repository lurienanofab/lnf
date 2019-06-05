using LNF.Models;
using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class ResourceManager : ApiClient, IResourceManager
    {
        public IResource GetResource(int resourceId)
        {
            return Get<ResourceItem>("webapi/scheduler/resource/{resourceId}", UrlSegments(new { resourceId }));
        }

        public IEnumerable<ListItem> AllActiveResources()
        {
            return Get<List<ListItem>>("webapi/scheduler/resource/active/list");
        }

        public string GetInterlockStatus(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceActivityAuth> GetResourceActivityAuths(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceCost> GetResourceCosts(DateTime? cutoff = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceCost> GetResourceCosts(int resourceId, DateTime? cutoff = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> GetToolEngineers(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResource> SelectActive()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResource> SelectByLab(int? labId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnCancelClients(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnOpeningClients(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnPracticeRes(int resourceId)
        {
            throw new NotImplementedException();
        }

        public int[] GetOffsets(int granularity)
        {
            return Get<List<int>>("webapi/scheduler/resource/offset", QueryStrings(new { granularity })).ToArray();
        }

        public IEnumerable<ReservationTime> GetMinReservationTime(int granularity)
        {
            return Get<List<ReservationTime>>("webapi/scheduler/resource/min-reservation-time", QueryStrings(new { granularity }));
        }

        public int[] GetMaxReservationTime(int granularity, int minReservTime)
        {
            return Get<List<int>>("webapi/scheduler/resource/max-reservation-time", QueryStrings(new { granularity, minReservTime })).ToArray();
        }

        public int[] GetGracePeriodHour(int granularity, int minReservTime)
        {
            return Get<List<int>>("webapi/scheduler/resource/grace-period-hour", QueryStrings(new { granularity, minReservTime })).ToArray();
        }

        public int[] GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour)
        {
            return Get<List<int>>("webapi/scheduler/resource/grace-period-minute", QueryStrings(new { granularity, minReservTime, gracePeriodHour })).ToArray();
        }
    }
}
