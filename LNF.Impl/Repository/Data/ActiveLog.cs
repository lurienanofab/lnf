using LNF.CommonTools;
using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// Used to track the active status of various items to determine if an item was active during a certain date range
    /// </summary>
    public class ActiveLog : IDataItem
    {
        /// <summary>
        /// The unique id of an ActiveLog
        /// </summary>
        public virtual int LogID { get; set; }

        /// <summary>
        /// The table name of the tracked item
        /// </summary>
        public virtual string TableName { get; set; }

        /// <summary>
        /// The unique id of the tracked item
        /// </summary>
        public virtual int Record { get; set; }

        /// <summary>
        /// The date the tracked item was enabled
        /// </summary>
        public virtual DateTime EnableDate { get; set; }

        /// <summary>
        /// The date the tracked item was disabled - null if the item is currenlty active
        /// </summary>
        public virtual DateTime? DisableDate { get; set; }


        /// <summary>
        /// Get a value that idicates if the item was active during a specified date range
        /// </summary>
        /// <param name="startDate">The start of the date range</param>
        /// <param name="endDate">The end of the date range</param>
        /// <returns>True if the item was active during the date range, otherwise false</returns>
        public virtual bool IsOverlapped(DateTime startDate, DateTime endDate)
        {
            return Utility.Overlap(EnableDate, DisableDate, startDate, endDate);
        }
    }
}
