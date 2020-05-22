using LNF.Cache;
using LNF.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public abstract class TreeItemCollection
    {
        protected IEnumerable<IResourceTree> _resources;

        public TreeItemCollection(IEnumerable<IResourceTree> resources)
        {
            //_resources = provider.Scheduler.Resource.GetResourceTree(client.ClientID);
            _resources = resources;
        }

        public int Count => _resources.Count();

        public IEnumerable<IResourceTree> Resources() => _resources.OrderBy(x => x.LabID).ToList();

        public IResource GetResource(int resourceId)
        {
            var result = GetResourceTree(resourceId);
            return result;
        }

        public IEnumerable<IBuilding> Buildings()
        {
            var distinct = Resources().Select(x => new
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

        public IEnumerable<ILab> Labs()
        {
            var distinct = Resources().Select(x => new
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

        public IEnumerable<IProcessTech> ProcessTechs()
        {
            var distinct = Resources().Select(x => new
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

        public IResourceTree GetResourceTree(int resourceId)
        {
            var result = Resources().FirstOrDefault(x => x.ResourceID == resourceId);
            if (result == null)
                throw new Exception($"Could not find a resource with ResourceID {resourceId}.");
            return result;
        }

        public IBuilding GetBuilding(int buildingId)
        {
            var result = Buildings().FirstOrDefault(x => x.BuildingID == buildingId);
            if (result == null)
                throw new Exception($"Could not find a building with BuildingID {buildingId}.");
            return result;
        }

        public ILab GetLab(int labId)
        {
            var result = Labs().FirstOrDefault(x => x.LabID == labId);
            if (result == null)
                throw new Exception($"Could not find a lab with LabID {labId}.");
            return result;
        }

        public IProcessTech GetProcessTech(int procTechId)
        {
            var result = ProcessTechs().FirstOrDefault(x => x.ProcessTechID == procTechId);
            if (result == null)
                throw new Exception($"Could not find a process tech with ProcessTechID {procTechId}.");
            return result;
        }

        public IActivity GetCurrentActivity(int resourceId)
        {
            var item = Resources().FirstOrDefault(x => x.ResourceID == resourceId);
            if (item == null) return null;
            if (item.CurrentActivityID == 0) return null;
            var result = CacheManager.Current.GetActivity(item.CurrentActivityID);
            return result;
        }

        public IClient GetCurrentClient(int resourceId)
        {
            var item = Resources().FirstOrDefault(x => x.ResourceID == resourceId);
            if (item == null) return null;
            if (item.CurrentClientID == 0) return null;
            var result = ServiceProvider.Current.Data.Client.GetClient(item.CurrentClientID);
            return result;
        }
    }

    public class LocationTreeItemCollection : TreeItemCollection
    {
        private readonly IEnumerable<ILabLocation> _labLocations;
        private readonly IEnumerable<IResourceLabLocation> _resourceLabLocations;

        public LocationTreeItemCollection(IEnumerable<IResourceTree> resources, IEnumerable<ILabLocation> labLocations, IEnumerable<IResourceLabLocation> resourceLabLocations) : base(resources)
        {
            _labLocations = labLocations ?? throw new ArgumentNullException("labLocations");
            _resourceLabLocations = resourceLabLocations ?? throw new ArgumentNullException("resourceLabLocations");
        }

        public IEnumerable<ILabLocation> GetLabLocations() => _labLocations;

        public IEnumerable<ILabLocation> GetLabLocations(int labId) => GetLabLocations().Where(x => x.LabID == labId);

        public IEnumerable<IResourceLabLocation> GetResourceLabLocations() => _resourceLabLocations;
    }

    public class ResourceTreeItemCollection : TreeItemCollection
    {
        public ResourceTreeItemCollection(IEnumerable<IResourceTree> resources) : base(resources) { }
    }
}
