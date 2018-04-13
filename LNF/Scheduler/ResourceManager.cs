using LNF.CommonTools;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LNF.Scheduler
{
    public class ResourceManager : ManagerBase
    {
        public ResourceManager(ISession session) : base(session) { }

        public Resource GetResource(ResourceModel item)
        {
            var result = Session.Single<Resource>(item.ResourceID);

            if (result == null)
                throw new InvalidOperationException(string.Format("No Resource found for ResourceID = {0}", item.ResourceID));

            return result;
        }

        public IQueryable<ResourceClientInfo> GetResourceClients(ResourceModel item)
        {
            return ResourceClientInfoUtility.GetResourceClients(item.ResourceID);
        }

        public IQueryable<ResourceClientInfo> GetToolEngineers(ResourceModel item)
        {
            return ResourceClientInfoUtility.GetToolEngineers(item.ResourceID);
        }

        public IQueryable<ResourceClientInfo> SelectNotifyOnCancelClients(ResourceModel item)
        {
            return ResourceClientInfoUtility.SelectNotifyOnCancelClients(item.ResourceID);
        }

        public IQueryable<ResourceClientInfo> SelectNotifyOnOpeningClients(ResourceModel item)
        {
            return ResourceClientInfoUtility.SelectNotifyOnOpeningClients(item.ResourceID);
        }

        public IQueryable<ResourceClientInfo> SelectNotifyOnPracticeRes(ResourceModel item)
        {
            return ResourceClientInfoUtility.SelectNotifyOnPracticeRes(item.ResourceID);
        }

        public IQueryable<ReservationRecurrence> GetReservationRecurrences(ResourceModel item)
        {
            return Session.Query<ReservationRecurrence>().Where(x => x.Resource.ResourceID == item.ResourceID);
        }

        public IQueryable<ResourceActivityAuth> GetResourceActivityAuths(ResourceModel item)
        {
            return Session.Query<ResourceActivityAuth>().Where(x => x.Resource.ResourceID == item.ResourceID);
        }

        public IList<ResourceCost> GetResourceCosts(ResourceModel item)
        {
            IEnumerable<Cost> costs = Session.Query<Cost>().Where(x => (x.RecordID == item.ResourceID && x.TableNameOrDescription == "ToolCost") || x.TableNameOrDescription == "ToolOvertimeCost");
            return ResourceCost.GetAll(costs, item.ResourceID);
        }

        public double SelectReservableMinutes(ResourceModel item, int clientId, DateTime now)
        {
            return DA.Current.ReservationManager().SelectReservableMinutes(item.ResourceID, clientId, item.ReservFence, item.MaxAlloc, now);
        }

        public async Task<string> GetInterlockStatus(ResourceModel item)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("InterlockStatus", typeof(string));
            dt.Rows.Add(item.ResourceID, string.Empty);
            await WagoInterlock.AllToolStatus(dt);
            DataRow dr = dt.Select(string.Format("ResourceID = {0}", item.ResourceID)).FirstOrDefault();
            string result = dr["InterlockStatus"].ToString();
            return result;
        }

        public DateTime? OpenResSlot(ResourceModel item, DateTime now, DateTime sd)
        {
            return DA.Current.ReservationManager().OpenResSlot(item.ResourceID, item.ReservFence, item.MinReservTime, now, sd);
        }

        /// <summary>
        /// Returns the next grain boundary in the past or future
        /// </summary>
        /// <param name="actualTime">The point in time to determine the next or previous granularity</param>
        /// <param name="granDir">The direction (next or pervious) to search in</param>
        /// <returns>The DateTime value of the next or previous granularity</returns>
        public DateTime GetNextGranularity(ResourceModel item, DateTime actualTime, NextGranDir granDir)
        {
            return ResourceUtility.GetNextGranularity(item.Granularity, item.Offset, actualTime, granDir);
        }

        /// <summary>
        /// Sets the start and end time slot boundaries
        /// </summary>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        public void GetTimeSlotBoundary(ResourceModel item, ref DateTime startTime, ref DateTime endTime)
        {
            ResourceUtility.GetTimeSlotBoundary(item.Granularity, item.Offset, ref startTime, ref endTime);
        }
    }
}
