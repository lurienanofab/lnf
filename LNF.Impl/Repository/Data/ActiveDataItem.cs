using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// A base class for items that have entries in ActiveLog
    /// </summary>
    public abstract class ActiveDataItem : IActiveDataItem
    {
        /// <summary>
        /// Indicates if the item is currently active
        /// </summary>
        public abstract bool Active { get; set; }

        /// <summary>
        /// The table name used in ActiveLog
        /// </summary>
        public abstract string TableName();

        /// <summary>
        /// The unique id of the item used in ActiveLog
        /// </summary>
        /// <returns></returns>
        public abstract int Record();
    }
}
