using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    /// <summary>
    /// Represents a room entry event from the physical access system
    /// </summary>
    public class RoomDataClean : IDataItem
    {
        /// <summary>
        /// The unique id of a RoomDataClean
        /// </summary>
        public virtual int RoomDataID { get; set; }

        /// <summary>
        /// The Client whose card was used to access the room
        /// </summary>
        public virtual int ClientID { get; set; }

        /// <summary>
        /// The Room that was accessed
        /// </summary>
        public virtual int RoomID { get; set; }

        /// <summary>
        /// The date/time the user entered the room
        /// </summary>
        public virtual DateTime EntryDT { get; set; }

        /// <summary>
        /// The date/time the user exited the room
        /// </summary>
        public virtual DateTime? ExitDT { get; set; }

        /// <summary>
        /// The length of time the user spent in the room
        /// </summary>
        public virtual double Duration { get; set; }
    }
}
