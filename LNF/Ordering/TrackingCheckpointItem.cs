namespace LNF.Ordering
{
    public class TrackingCheckpointItem : ITrackingCheckpoint
    {
        public int TrackingCheckpointID { get; set; }
        public string CheckpointName { get; set; }
        public bool Active { get; set; }
    }
}
