using LNF.Models.Inventory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineServices.Api.Inventory
{
    public class InventoryClient : ApiClient
    {
        internal InventoryClient(ApiClientOptions options) : base(options) { }

        public async Task<IEnumerable<CategoryModel>> GetCategories()
        {
            return await Get<IEnumerable<CategoryModel>>("inventory/category");
        }

        public async Task<CategoryModel> GetCategory(int catId)
        {
            return await Get<CategoryModel>(string.Format("inventory/category/{0}", catId));
        }

        public async Task<IEnumerable<InventoryTypeModel>> GetInventoryTypes()
        {
            return await Get<IEnumerable<InventoryTypeModel>>("inventory/inventory-type");
        }

        public async Task<InventoryTypeModel> GetInventoryType(int inventoryTypeId)
        {
            return await Get<InventoryTypeModel>(string.Format("inventory/inventory-type/{0}", inventoryTypeId));
        }

        public async Task<IEnumerable<ItemModel>> GetItems()
        {
            return await Get<IEnumerable<ItemModel>>("inventory/item");
        }

        public async Task<ItemModel> GetItem(int itemId)
        {
            return await Get<ItemModel>(string.Format("inventory/item/{0}", itemId));
        }
    }
}
