using System;

namespace LNF.Ordering
{
    public class TrackingItem : ITracking
    {
        public int TrackingID { get; set; }
        public int TrackingCheckpointID { get; set; }
        public int POID { get; set; }
        public int ClientID { get; set; }
        public string TrackingData { get; set; }
        public DateTime TrackingDateTime { get; set; }
        public string CheckpointName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
    }
}
