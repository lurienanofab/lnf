using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
namespace LNF.Impl.Scheduler
{
    public class LabLocationRepository : RepositoryBase, ILabLocationRepository
    {
        public LabLocationRepository(ISessionManager mgr) : base(mgr) { }

        public ILabLocation GetLabLocation(int labLocationId)
        {
            return Session.Get<LabLocation>(labLocationId);
        }

        public ILabLocation GetLabLocationByResource(int resourceId)
        {
            ResourceLabLocation resourceLabLocation = null;
            LabLocation labLocation = null;

            var result = Session.QueryOver(() => labLocation)
                .JoinEntityAlias(() => resourceLabLocation, () => resourceLabLocation.LabLocationID == labLocation.LabLocationID)
                .Where(() => resourceLabLocation.ResourceID == resourceId)
                .SingleOrDefault();

            return result;
        }

        public IEnumerable<ILabLocation> GetLabLocations()
        {
            return Session.Query<LabLocation>().ToList();
        }

        public IEnumerable<ILabLocation> GetLabLocations(int labId)
        {
            return Session.Query<LabLocation>().Where(x => x.LabID == labId).ToList();
        }

        public IResourceLabLocation GetResourceLabLocation(int resourceLabLocationId)
        {
            return Session.Get<ResourceLabLocation>(resourceLabLocationId);
        }

        public IResourceLabLocation GetResourceLabLocationByResource(int resourceId)
        {
            return Session.Query<ResourceLabLocation>().FirstOrDefault(x => x.ResourceID == resourceId);
        }

        public IEnumerable<IResourceLabLocation> GetResourceLabLocations(int labLocationId)
        {
            return Session.Query<ResourceLabLocation>().Where(x => x.LabLocationID == labLocationId).ToList();
        }

        public IEnumerable<IResource> GetResources(int labLocationId)
        {
            var locactions = Session.Query<ResourceLabLocation>().Where(x => x.LabLocationID == labLocationId);
            var resources = locactions.Join(Session.Query<ResourceInfo>(), o => o.ResourceID, i => i.ResourceID, (o, i) => i);
            var result = resources.ToList();
            return result;
            throw new NotImplementedException();
        }
    }
}
