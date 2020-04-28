using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Ordering;
using LNF.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Ordering
{
    public class TrackingRepository : RepositoryBase, ITrackingRepository
    {
        public TrackingRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<ITracking> GetTracking(int poid)
        {
            return Session.Query<Repository.Ordering.Tracking>().Where(x => x.PurchaseOrder.POID == poid).CreateModels<ITracking>();
        }

        public ITracking AddTracking(TrackingCheckpoints checkpoint, int poid, int clientId, string data)
        {
            var tracking = new Repository.Ordering.Tracking
            {
                TrackingCheckpoint = RequireCheckpoint(checkpoint),
                PurchaseOrder = Require<PurchaseOrder>(poid),
                Client = Require<Client>(clientId),
                TrackingData = data,
                TrackingDateTime = DateTime.Now
            };

            Session.Save(tracking);

            return tracking.CreateModel<ITracking>();
        }

        public IEnumerable<ITrackingCheckpoint> GetCheckpoints()
        {
            return Session.Query<TrackingCheckpoint>().CreateModels<ITrackingCheckpoint>();
        }

        public ITrackingCheckpoint GetCheckpoint(TrackingCheckpoints checkpoint)
        {
            return RequireCheckpoint(checkpoint).CreateModel<ITrackingCheckpoint>();
        }

        private TrackingCheckpoint RequireCheckpoint(TrackingCheckpoints checkpoint)
        {
            return Require<TrackingCheckpoint>(Convert.ToInt32(checkpoint));
        }
    }
}
