using System;

namespace LNF.Repository.Data
{
    /// <summary>
    /// A readonly view of a Client (system user) - all related tables are joined to retrieve all information with one query, including ActiveLog records
    /// </summary>
    public class ActiveLogClient : ClientOrgInfoBase, IActiveLogItem, IDataItem
    {
        /// <summary>
        /// The unique id of an ActiveLog
        /// </summary>
        public virtual int LogID { get; set; }

        /// <summary>
        /// The id of the associated object - same as AccountID
        /// </summary>
        public virtual int Record { get; set; }

        /// <summary>
        /// The date the object was enabled
        /// </summary>
        public virtual DateTime EnableDate { get; set; }

        /// <summary>
        /// The date the object was disabled, or null if the object is currently active
        /// </summary>
        public virtual DateTime? DisableDate { get; set; }

        /// <summary>
        /// Gets a value that indicates if the item is active, as well as any related items
        /// </summary>
        /// <returns>True if the Org and Client are both active and DisableDate is null, otherwise false</returns>
        public override bool IsActive()
        {
            return ClientActive && DisableDate == null;
        }
    }
}
