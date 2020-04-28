namespace LNF.DataAccess
{
    /// <summary>
    /// Represents an item that has an Active state tracked in ActiveLog
    /// </summary>
    public interface IActiveDataItem : IDataItem
    {
        /// <summary>
        /// The current Active state of the item
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// The item id used in ActiveLog
        /// </summary>
        /// <returns></returns>
        int Record();

        /// <summary>
        /// The item table name used in ActiveLog
        /// </summary>
        /// <returns></returns>
        string TableName();
    }
}
