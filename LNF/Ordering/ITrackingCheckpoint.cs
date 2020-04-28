namespace LNF.Ordering
{
    public interface ITrackingCheckpoint
    {
        /// <summary>
        /// The unique id of a TrackingCheckpoint
        /// </summary>
        int TrackingCheckpointID { get; set; }

        /// <summary>
        /// The name of a TrackingCheckpoint
        /// </summary>
        string CheckpointName { get; set; }

        /// <summary>
        /// Indicates if the the TrackingCheckpoint is active
        /// </summary>
        bool Active { get; set; }
    }
}
