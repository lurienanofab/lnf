using LNF.Cache;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ResourceTreeItemCollection : IEnumerable<IResourceTree>
    {
        private IEnumerable<IResourceTree> _items;

        public ResourceTreeItemCollection(IEnumerable<IResourceTree> items)
        {
            _items = items;
        }

        public int Count
        {
            get { return _items.Count(); }
        }

        public IEnumerable<BuildingItem> Buildings()
        {
            var distinct = _items.Select(x => new
            {
                x.BuildingID,
                x.BuildingName,
                x.BuildingDescription,
                x.BuildingIsActive
            }).Distinct();

            var result = distinct.OrderBy(x => x.BuildingID).Select(x => new BuildingItem()
            {
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingDescription = x.BuildingDescription,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public IEnumerable<LabItem> Labs()
        {
            var distinct = _items.Select(x => new
            {
                x.LabID,
                x.LabName,
                x.LabDisplayName,
                x.LabDescription,
                x.LabIsActive,
                x.BuildingID,
                x.BuildingName,
                x.BuildingIsActive
            }).Distinct();

            var result = distinct.OrderBy(x => x.LabID).Select(x => new LabItem()
            {
                LabID = x.LabID,
                LabName = x.LabName,
                LabDisplayName = x.LabDisplayName,
                LabDescription = x.LabDescription,
                LabIsActive = x.LabIsActive,
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingIsActive = x.BuildingIsActive
            }).ToList();

            return result;
        }

        public IList<ProcessTechItem> ProcessTechs()
        {
            var distinct = _items.Select(x => new
            {
                x.ProcessTechID,
                x.ProcessTechName,
                x.ProcessTechDescription,
                x.ProcessTechIsActive,
                x.ProcessTechGroupID,
                x.ProcessTechGroupName,
                x.LabID,
                x.LabName,
                x.LabDisplayName,
                x.LabDescription,
                x.LabIsActive,
                x.BuildingID,
                x.BuildingName,
                x.BuildingIsActive
            }).Distinct();

            var result = distinct.OrderBy(x => x.LabID).Select(x => new ProcessTechItem()
            {
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                ProcessTechDescription = x.ProcessTechDescription,
                ProcessTechIsActive = x.ProcessTechIsActive,
                ProcessTechGroupID = x.ProcessTechGroupID,
                ProcessTechGroupName = x.ProcessTechGroupName,
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

        public IEnumerable<IResourceTree> Resources()
        {
            var result = _items.OrderBy(x => x.LabID).ToList();
            return result;
        }

        public IResource GetResource(int resourceId)
        {
            var result = Find(resourceId);
            return result;
        }

        public IResourceTree Find(int resourceId)
        {
            var result = _items.FirstOrDefault(x => x.ResourceID == resourceId);
            if (result == null)
                throw new Exception($"Could not find a resource with ResourceID {resourceId}.");
            return result;
        }

        public BuildingItem GetBuilding(int buildingId)
        {
            var result = Buildings().FirstOrDefault(x => x.BuildingID == buildingId);
            if (result == null)
                throw new Exception($"Could not find a building with BuildingID {buildingId}.");
            return result;
        }

        public LabItem GetLab(int labId)
        {
            var result = Labs().FirstOrDefault(x => x.LabID == labId);
            if (result == null)
                throw new Exception($"Could not find a lab with LabID {labId}.");
            return result;
        }

        public ProcessTechItem GetProcessTech(int procTechId)
        {
            var result = ProcessTechs().FirstOrDefault(x => x.ProcessTechID == procTechId);
            if (result == null)
                throw new Exception($"Could not find a process tech with ProcessTechID {procTechId}.");
            return result;
        }

        public IActivity GetCurrentActivity(int resourceId)
        {
            var item = _items.FirstOrDefault(x => x.ResourceID == resourceId);
            if (item == null) return null;
            if (item.CurrentActivityID == 0) return null;
            var result = CacheManager.Current.GetActivity(item.CurrentActivityID);
            return result;
        }

        public IClient GetCurrentClient(int resourceId)
        {
            var item = _items.FirstOrDefault(x => x.ResourceID == resourceId);
            if (item == null) return null;
            if (item.CurrentClientID == 0) return null;
            var result = ServiceProvider.Current.Data.Client.GetClient(item.CurrentClientID);
            return result;
        }

        public IEnumerator<IResourceTree> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
