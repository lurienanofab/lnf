namespace LNF.Inventory
{
    public interface IInventoryService
    {
        IInventoryItemRepository Item { get; }
        ICategoryRepository Category { get; }
    }
}
