using LNF.Inventory;
using RestSharp;

namespace OnlineServices.Api.Inventory
{
    public class InventoryService : IInventoryService
    {
        public IInventoryItemRepository Item { get; }

        public ICategoryRepository Category { get; }

        internal InventoryService(IRestClient rc)
        {
            Item = new InventoryItemRepository(rc);
            Category = new CategoryRepository(rc);
        }
    }
}
