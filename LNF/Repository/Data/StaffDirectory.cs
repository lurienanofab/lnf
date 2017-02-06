using System;

namespace LNF.Repository.Data
{
    /// <summary>
    /// A staff directory entry
    /// </summary>
    public class StaffDirectory : IDataItem
    {
        /// <summary>
        /// The unique id of a staff directory entry
        /// </summary>
        public virtual int StaffDirectoryID { get; set; }

        /// <summary>
        /// The Client item of the staff associated with an entry
        /// </summary>
        public virtual Client Client { get; set; }

        /// <summary>
        /// An XML representation of the staff work hours
        /// </summary>
        public virtual string HoursXML { get; set; }

        /// <summary>
        /// The staff contact phone
        /// </summary>
        public virtual string ContactPhone { get; set; }

        /// <summary>
        /// The staff office location (building and room)
        /// </summary>
        public virtual string Office { get; set; }

        /// <summary>
        /// Indicates if an entry has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// The date and time when an entry was last modified
        /// </summary>
        public virtual DateTime LastUpdate { get; set; }
    }
}
