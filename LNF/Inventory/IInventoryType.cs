namespace LNF.Inventory
{
    /// <summary>
    /// The highest level of categorization for inventory items. All items must belong to at least one type
    /// </summary>
    public interface IInventoryType
    {
        /// <summary>
        /// The id for the inventory type
        /// </summary>
        int InventoryTypeID { get; set; }

        /// <summary>
        /// The name of the inventory type
        /// </summary>
        string InventoryTypeName { get; set; }

        /// <summary>
        /// Indicates whether or not the inventory type has been deleted
        /// </summary>
        bool Deleted { get; set; }
    }
}