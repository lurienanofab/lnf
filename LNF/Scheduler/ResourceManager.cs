﻿using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LNF.Scheduler
{
    public class ResourceManager : ManagerBase, IResourceManager
    {
        protected IContext Context { get; }
        protected ICostManager CostManager { get; }

        public ResourceManager(ISession session, IContext context, ICostManager costManager) : base(session)
        {
            Context = context;
            CostManager = costManager;
        }

        public IQueryable<Resource> SelectActive()
        {
            return Session.Query<Resource>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.ResourceName);
        }

        public IQueryable<Resource> SelectByLab(int? labId)
        {
            //procResourceSelect @Action='SelectByLabID'

            /*if @LabID is null
                Select ResourceName, ResourceID
                from Resource
                where IsActive = 1
                and LabID in (1, 9)	
			    order by ResourceName
		    else
			    Select ResourceName, ResourceID
                from Resource
                where IsActive = 1
                and LabID = case when @LabID = 0 then LabID else @LabID end
                order by ResourceName*/

            int[] defaultLabs = { 1, 9 };

            if (ConfigurationManager.AppSettings["DefaultLabs"] != null)
                defaultLabs = ConfigurationManager.AppSettings["DefaultLabs"].Split(',').Select(int.Parse).ToArray();

            IQueryable<Resource> query;

            if (labId == null)
                query = Session.Query<Resource>().Where(x => x.IsActive && defaultLabs.Contains(x.ProcessTech.Lab.LabID));
            else
            {
                if (labId.Value == 0)
                    query = Session.Query<Resource>().Where(x => x.IsActive);
                else
                    query = Session.Query<Resource>().Where(x => x.IsActive && x.ProcessTech.Lab.LabID == labId.Value);
            }

            var result = query.OrderBy(x => x.ProcessTech.Lab.Building.BuildingName).ThenBy(x => x.ProcessTech.Lab.LabName).ThenBy(x => x.ProcessTech.ProcessTechName).ThenBy(x => x.ResourceName);

            return result;
        }

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

        public IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId)
        {
            string key = "ResourceCosts#" + resourceId.ToString();

            IList<ResourceCostModel> result = Context.GetItem<IList<ResourceCostModel>>(key);

            if (result == null || result.Count == 0)
            {
                var costs = CostManager.FindCosts("ToolCost", cutoff, null, resourceId);
                result = CreateResourceCostModels(costs).ToList();
                Context.SetItem(key, result);
            }

            return result;
        }

        public IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId, int chargeTypeId)
        {
            return GetToolCosts(cutoff, resourceId).Where(x => x.ChargeTypeID == chargeTypeId).ToList();
        }

        private IEnumerable<ResourceCostModel> CreateResourceCostModels(IEnumerable<CostModel> source)
        {
            return source.Select(x => new ResourceCostModel()
            {
                ResourceID = x.RecordID,
                ChargeTypeID = x.ChargeTypeID,
                ChargeTypeName = x.ChargeTypeName,
                AddVal = x.AddVal,
                MulVal = x.MulVal
            }).ToList();
        }
    }
}
