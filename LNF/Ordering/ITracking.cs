using System;

namespace LNF.Ordering
{
    public interface ITracking
    {
        int TrackingID { get; set; }
        int TrackingCheckpointID { get; set; }
        int POID { get; set; }
        int ClientID { get; set; }
        string TrackingData { get; set; }
        DateTime TrackingDateTime { get; set; }
        string CheckpointName { get; set; }
        string LName { get; set; }
        string FName { get; set; }
    }
}
