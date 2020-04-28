namespace LNF.Inventory
{
    public class InventoryService : IInventoryService
    {
        public IInventoryItemRepository Item { get; }
        public ICategoryRepository Category { get; }

        public InventoryService(IInventoryItemRepository item, ICategoryRepository category)
        {
            Item = item;
            Category = category;
        }
    }
}
