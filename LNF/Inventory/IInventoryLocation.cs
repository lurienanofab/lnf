namespace LNF.Inventory
{
    public interface IInventoryLocation
    {
        int InventoryLocationID { get; set; }
        int ParentID { get; set; }
        string LocationName { get; set; }
        bool Active { get; set; }
        bool IsStoreLocation { get; set; }
        string LocationType { get; set; }
    }
}
