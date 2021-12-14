using LNF.Ordering;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Ordering
{
    public class TrackingRepository : ApiClient, ITrackingRepository
    {
        internal TrackingRepository(IRestClient rc) : base(rc) { }

        public ITracking AddTracking(TrackingCheckpoints checkpoint, int poid, int clientId, string data)
        {
            throw new NotImplementedException();
        }

        public ITrackingCheckpoint GetCheckpoint(TrackingCheckpoints checkpoint)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITrackingCheckpoint> GetCheckpoints()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITracking> GetTracking(int poid)
        {
            throw new NotImplementedException();
        }
    }
}
