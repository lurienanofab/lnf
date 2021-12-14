using LNF.Inventory;
using RestSharp;
using System.Collections.Generic;

namespace OnlineServices.Api.Inventory
{
    public class InventoryClient : ApiClient
    {
        internal InventoryClient(IRestClient rc) : base(rc) { }

        public IEnumerable<Category> GetCategories()
        {
            return Get<List<Category>>("webapi/inventory/category");
        }

        public Category GetCategory(int catId)
        {
            return Get<Category>("webapi/inventory/category/{catId}", UrlSegments(new { catId }));
        }

        public IEnumerable<InventoryType> GetInventoryTypes()
        {
            return Get<List<InventoryType>>("webapi/inventory/inventory-type");
        }

        public InventoryType GetInventoryType(int inventoryTypeId)
        {
            return Get<InventoryType>("webapi/inventory/inventory-type/{inventoryTypeId}", UrlSegments(new { inventoryTypeId }));
        }

        public IEnumerable<IInventoryItem> GetItems()
        {
            return Get<List<InventoryItem>>("webapi/inventory/item");
        }

        public IInventoryItem GetItem(int itemId)
        {
            return Get<InventoryItem>("webapi/inventory/item/{itemId}", UrlSegments(new { itemId }));
        }
    }
}
