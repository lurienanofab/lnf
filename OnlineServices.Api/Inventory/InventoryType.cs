using LNF.Inventory;

namespace OnlineServices.Api.Inventory
{
    public class InventoryType : IInventoryType
    {
        public int InventoryTypeID { get; set; }
        public string InventoryTypeName { get; set; }
        public bool Deleted { get; set; }
    }
}
