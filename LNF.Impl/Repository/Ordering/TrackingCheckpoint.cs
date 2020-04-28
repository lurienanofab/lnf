using LNF.DataAccess;
using LNF.Ordering;
using System;

namespace LNF.Impl.Repository.Ordering
{
    /// <summary>
    /// A checkpoint used for tracking an IOF
    /// </summary>
    public class TrackingCheckpoint : IDataItem
    {
        /// <summary>
        /// The unique id of a TrackingCheckpoint
        /// </summary>
        public virtual int TrackingCheckpointID { get; set; }

        /// <summary>
        /// The name of a TrackingCheckpoint
        /// </summary>
        public virtual string CheckpointName { get; set; }

        /// <summary>
        /// Indicates if the the TrackingCheckpoint is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Get a value that indicates if the TrackingCheckpoint corresponds to the specified TrackingCheckpoints enum value
        /// </summary>
        /// <param name="checkpoint">An enum value for tracking checkpoints</param>
        /// <returns>True if the TrackingCheckpoint corresponds to the specified TrackingCheckpoints enum value, otherwise false</returns>
        public virtual bool Is(TrackingCheckpoints checkpoint)
        {
            return TrackingCheckpointID == Convert.ToInt32(checkpoint);
        }
    }
}
