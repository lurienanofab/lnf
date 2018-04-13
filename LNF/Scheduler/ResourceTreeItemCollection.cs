using LNF.Cache;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ResourceTreeItemCollection : IEnumerable<ResourceTreeItem>
    {
        private IList<ResourceTreeItem> _items;

        public ResourceTreeItemCollection(IEnumerable<ResourceTreeItem> items)
        {
            _items = items.ToList();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public IEnumerable<BuildingModel> Buildings()
        {
            var distinct = _items.Select(x => new { x.BuildingID, x.BuildingName, x.BuildingDescription, x.BuildingIsActive }).Distinct();

            var result = distinct.OrderBy(x => x.BuildingID).Select(x => new BuildingModel()
            {
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingDescription = x.BuildingDescription,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public IEnumerable<LabModel> Labs()
        {
            var distinct = _items.Select(x => new { x.LabID, x.LabName, x.LabDisplayName, x.LabDescription, x.LabIsActive, x.RoomID, x.RoomName, x.BuildingID, x.BuildingName, x.BuildingIsActive }).Distinct();

            var result = distinct.OrderBy(x => x.LabID).Select(x => new LabModel()
            {
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                LabDescription = x.LabDescription,
                LabIsActive = x.LabIsActive,
                RoomID = x.RoomID,
                RoomName = x.RoomName,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public IList<ProcessTechModel> ProcessTechs()
        {
            var distinct = _items.Select(x => new { x.ProcessTechID, x.ProcessTechName, x.ProcessTechDescription, x.ProcessTechIsActive, x.ProcessTechGroupID, x.ProcessTechGroupName, x.LabID, x.LabName, x.LabDisplayName, x.LabDescription, x.LabIsActive, x.RoomID, x.RoomName, x.BuildingID, x.BuildingName, x.BuildingIsActive }).Distinct();

            var result = distinct.OrderBy(x => x.LabID).Select(x => new ProcessTechModel()
            {
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                ProcessTechDescription = x.ProcessTechDescription,
                ProcessTechIsActive = x.ProcessTechIsActive,
                GroupID = x.ProcessTechGroupID,
                GroupName = x.ProcessTechGroupName,
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                LabIsActive = x.LabIsActive,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public IEnumerable<ResourceModel> Resources()
        {
            var result = CreateResourceModels(_items).OrderBy(x => x.LabID).ToList();
            return result;
        }

        public ResourceModel GetResource(int resourceId)
        {
            return CreateResourceModels(_items.Where(x => x.ResourceID == resourceId)).First();
        }

        public BuildingModel GetBuilding(int buildingId)
        {
            return Buildings().First(x => x.BuildingID == buildingId);
        }

        public LabModel GetLab(int labId)
        {
            return Labs().First(x => x.LabID == labId);
        }

        public ProcessTechModel GetProcessTech(int procTechId)
        {
            return ProcessTechs().First(x => x.ProcessTechID == procTechId);
        }

        public ActivityModel GetCurrentActivity(int resourceId)
        {
            ResourceTreeItem item = _items.Where(x => x.ResourceID == resourceId).FirstOrDefault();
            if (item == null) return null;
            if (item.CurrentActivityID == 0) return null;
            var result = CacheManager.Current.GetActivity(item.CurrentActivityID);
            return result;
        }

        public ClientItem GetCurrentClient(int resourceId)
        {
            ResourceTreeItem item = _items.Where(x => x.ResourceID == resourceId).FirstOrDefault();
            if (item == null) return null;
            if (item.CurrentClientID == 0) return null;
            var result = CacheManager.Current.GetClient(item.CurrentClientID);
            return result;
        }

        public IEnumerator<ResourceTreeItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static IEnumerable<ResourceModel> CreateResourceModels(IEnumerable<ResourceTreeItem> items)
        {
            return items.Select(x => new ResourceModel()
            {
                AuthDuration = x.AuthDuration,
                AuthState = x.AuthState,
                AutoEnd = TimeSpan.FromMinutes(x.AutoEnd),
                GracePeriod = TimeSpan.FromMinutes(x.GracePeriod),
                Granularity = TimeSpan.FromMinutes(x.Granularity),
                HelpdeskEmail = x.HelpdeskEmail,
                IsReady = x.IsReady,
                IsSchedulable = x.IsSchedulable,
                MaxAlloc = TimeSpan.FromMinutes(x.MaxAlloc),
                MaxReservTime = TimeSpan.FromMinutes(x.MaxReservTime),
                MinCancelTime = TimeSpan.FromMinutes(x.MinCancelTime),
                MinReservTime = TimeSpan.FromMinutes(x.MinReservTime),
                Offset = TimeSpan.FromHours(x.Offset),
                ReservFence = TimeSpan.FromMinutes(x.ReservFence),
                ResourceID = x.ResourceID,
                ResourceIsActive = x.ResourceIsActive,
                ResourceName = x.ResourceName,
                ResourceDescription = x.ResourceDescription,
                State = x.State,
                StateNotes = x.StateNotes,
                UnloadTime = TimeSpan.FromMinutes(x.UnloadTime),
                WikiPageUrl = x.WikiPageUrl,
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName
            }).ToList();
        }
    }
}
