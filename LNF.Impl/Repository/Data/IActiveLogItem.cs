using System;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// An interface for classes that use the ActiveLog
    /// </summary>
    public interface IActiveLogItem
    {
        /// <summary>
        /// The unique ActiveLog LogID
        /// </summary>
        int LogID { get; set; }

        /// <summary>
        /// The unqiue id of the record
        /// </summary>
        int Record { get; set; }

        /// <summary>
        /// The date the record was enabled
        /// </summary>
        DateTime EnableDate { get; set; }

        /// <summary>
        /// The date the record was disabled - null if the record is active
        /// </summary>
        DateTime? DisableDate { get; set; }
    }
}
