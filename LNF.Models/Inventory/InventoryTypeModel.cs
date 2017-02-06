namespace LNF.Models.Inventory
{
    /// <summary>
    /// The highest level of categorization for inventory items. All items must belong to at least one type
    /// </summary>
    public class InventoryTypeModel
    {
        /// <summary>
        /// The id for the inventory type
        /// </summary>
        public int InventoryTypeID { get; set; }

        /// <summary>
        /// The name of the inventory type
        /// </summary>
        public string InventoryTypeName { get; set; }

        /// <summary>
        /// Indicates whether or not the inventory type has been deleted
        /// </summary>
        public bool Deleted { get; set; }
    }
}
