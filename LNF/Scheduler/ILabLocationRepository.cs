using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface ILabLocationRepository
    {
        ILabLocation GetLabLocation(int labLocationId);
        ILabLocation GetLabLocationByResource(int resourceId);
        IEnumerable<ILabLocation> GetLabLocations();
        IEnumerable<ILabLocation> GetLabLocations(int labId);
        IResourceLabLocation GetResourceLabLocation(int resourceLabLocationId);
        IResourceLabLocation GetResourceLabLocationByResource(int resourceId);
        IEnumerable<IResourceLabLocation> GetResourceLabLocations();
        IEnumerable<IResourceLabLocation> GetResourceLabLocations(int labLocationId);
        IEnumerable<IResource> GetResources(int labLocationId);
    }
}
