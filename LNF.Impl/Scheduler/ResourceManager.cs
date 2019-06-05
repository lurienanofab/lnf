using LNF.CommonTools;
using LNF.Mail;
using LNF.Models;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ResourceManager : ManagerBase, IResourceManager
    {
        public ResourceManager(IProvider provider) : base(provider) { }

        public IResource GetResource(int resourceId)
        {
            return Session.Single<ResourceInfo>(resourceId).CreateModel<IResource>();
        }

        public IEnumerable<IResource> SelectActive()
        {
            return Session.Query<ResourceInfo>()
                .Where(x => x.ResourceIsActive)
                .OrderBy(x => x.ResourceName)
                .CreateModels<IResource>();
        }

        public IEnumerable<IResource> SelectByLab(int? labId)
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

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultLabs"]))
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

            return result.CreateModels<IResource>();
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId)
        {
            return LNF.Scheduler.ResourceClientInfoUtility.GetResourceClients(resourceId).CreateModels<IResourceClient>();
        }

        public IEnumerable<IResourceClient> GetToolEngineers(int resourceId)
        {
            return LNF.Scheduler.ResourceClientInfoUtility.GetToolEngineers(resourceId).CreateModels<IResourceClient>();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnCancelClients(int resourceId)
        {
            return LNF.Scheduler.ResourceClientInfoUtility.SelectNotifyOnCancelClients(resourceId).CreateModels<IResourceClient>();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnOpeningClients(int resourceId)
        {
            return LNF.Scheduler.ResourceClientInfoUtility.SelectNotifyOnOpeningClients(resourceId).CreateModels<IResourceClient>();
        }

        public IEnumerable<IResourceClient> SelectNotifyOnPracticeRes(int resourceId)
        {
            return LNF.Scheduler.ResourceClientInfoUtility.SelectNotifyOnPracticeRes(resourceId).CreateModels<IResourceClient>();
        }

        public IEnumerable<IResourceActivityAuth> GetResourceActivityAuths(int resourceId)
        {
            return Session.Query<ResourceActivityAuth>()
                .Where(x => x.Resource.ResourceID == resourceId)
                .CreateModels<IResourceActivityAuth>();
        }

        public IEnumerable<IResourceCost> GetResourceCosts(DateTime? cutoff = null)
        {
            var costs = Provider.Data.Cost.FindToolCosts(cutoff);
            return ResourceCost.CreateResourceCosts(costs);
        }

        public IEnumerable<IResourceCost> GetResourceCosts(int resourceId, DateTime? cutoff = null)
        {
            var costs = Provider.Data.Cost.FindToolCosts(resourceId, cutoff);
            return ResourceCost.CreateResourceCosts(costs);
        }

        public string GetInterlockStatus(int resourceId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("InterlockStatus", typeof(string));
            dt.Rows.Add(resourceId, string.Empty);
            WagoInterlock.AllToolStatus(dt);
            DataRow dr = dt.Select($"ResourceID = {resourceId}").FirstOrDefault();

            string result;

            if (dr != null)
                result = Convert.ToString(dr["InterlockStatus"]);
            else
                result = $"{resourceId} not found";

            return result;
        }

        public IEnumerable<ListItem> AllActiveResources()
        {
            var dv = GroupEmailManager.GetAllActiveTools();

            var result = new List<ListItem>();

            foreach (DataRowView drv in dv)
            {
                result.Add(new ListItem(Convert.ToString(drv["ResourceID"]), Convert.ToString(drv["ResourceName"])));
            }

            return result;
        }

        public int[] GetOffsets(int granularity)
        {
            var result = new List<int> { 0 };

            if (granularity > 60)
                result.Add(1);

            if (granularity > 120)
                result.Add(2);

            return result.ToArray();
        }

        public IEnumerable<ReservationTime> GetMinReservationTime(int granularity)
        {
            // Load Hours
            var result = new List<ReservationTime>();

            for (int i = 1; i <= 6; i++)
            {
                double minReservTime = i * granularity;
                TimeSpan ts = TimeSpan.FromMinutes(minReservTime);
                double day, hour, minute;

                //hour = Math.Floor(minReservTime / 60);
                //minute = minReservTime % 60;
                day = ts.Days;
                hour = ts.Hours;
                minute = ts.Minutes;

                string text = string.Empty;

                if (day > 0) text += string.Format("{0} day ", day);
                if (hour > 0) text += string.Format("{0} hr ", hour);
                if (minute > 0) text += string.Format("{0} min ", minute);

                result.Add(ReservationTime.Create(minReservTime, text.Trim()));
            }

            return result;
        }

        public int[] GetMaxReservationTime(int granularity, int minReservTime)
        {
            //                                                                  1               2   3    6   12   24      days
            int[] maxReservTimeList = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 18, 24, 30, 36, 42, 48, 72, 144, 288, 576 }; //hours

            // the max is 576 because the max granularity is now 1440 (1440 * 24 / 60 = 576)

            int maxValue = Convert.ToInt32(granularity * 24 / 60);
            int minValue = Convert.ToInt32(Math.Ceiling((double)minReservTime / 60));

            var result = new List<int>();

            for (int i = 0; i < maxReservTimeList.Length; i++)
            {
                int h = maxReservTimeList[i];
                if (h > maxValue) break;
                if (h >= minValue && (h * 60) % granularity == 0)
                    result.Add(h);
            }

            return result.ToArray();
        }

        public int[] GetGracePeriodHour(int granularity, int minReservTime)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            var stepSize = Convert.ToInt32(Math.Ceiling((double)granularity / 60));

            int minValue = 0;

            if (granularity >= 60) minValue = stepSize;

            for (int i = minValue; i <= maxHour; i += stepSize)
            {
                result.Add(i);
            }

            return result.ToArray();
        }

        public int[] GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            if (gracePeriodHour == maxHour && granularity < 60)
            {
                var maxMinute = minReservTime % 60;
                for (int i = 0; i <= maxMinute; i += granularity)
                {
                    result.Add(i);
                }
            }
            else
            {
                var count = Convert.ToInt32(Math.Ceiling(60 / (double)granularity));
                for (int i = 0; i < count; i++)
                {
                    var minute = granularity * i;
                    result.Add(minute);
                }
            }

            return result.ToArray();
        }
    }
}
