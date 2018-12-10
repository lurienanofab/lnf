using LNF.Models.Inventory;
using System.Collections.Generic;

namespace OnlineServices.Api.Inventory
{
    public class InventoryClient : ApiClient
    {
        internal InventoryClient() : base(GetApiBaseUrl()) { }

        public IEnumerable<CategoryModel> GetCategories()
        {
            return Get<List<CategoryModel>>("webapi/inventory/category");
        }

        public CategoryModel GetCategory(int catId)
        {
            return Get<CategoryModel>("webapi/inventory/category/{catId}", UrlSegments(new { catId }));
        }

        public IEnumerable<InventoryTypeModel> GetInventoryTypes()
        {
            return Get<List<InventoryTypeModel>>("webapi/inventory/inventory-type");
        }

        public InventoryTypeModel GetInventoryType(int inventoryTypeId)
        {
            return Get<InventoryTypeModel>("webapi/inventory/inventory-type/{inventoryTypeId}", UrlSegments(new { inventoryTypeId }));
        }

        public IEnumerable<ItemModel> GetItems()
        {
            return Get<List<ItemModel>>("webapi/inventory/item");
        }

        public ItemModel GetItem(int itemId)
        {
            return Get<ItemModel>("webapi/inventory/item/{itemId}", UrlSegments(new { itemId }));
        }
    }
}
