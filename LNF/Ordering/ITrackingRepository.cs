using System.Collections.Generic;

namespace LNF.Ordering
{
    public interface ITrackingRepository
    {
        IEnumerable<ITracking> GetTracking(int poid);
        ITracking AddTracking(TrackingCheckpoints checkpoint, int poid, int clientId, string data);
        IEnumerable<ITrackingCheckpoint> GetCheckpoints();
        ITrackingCheckpoint GetCheckpoint(TrackingCheckpoints checkpoint);
    }
}
