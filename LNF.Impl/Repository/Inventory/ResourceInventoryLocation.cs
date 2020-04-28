using LNF.DataAccess;
using LNF.Impl.Repository.Scheduler;

namespace LNF.Impl.Repository.Inventory
{
    public class ResourceInventoryLocation : IDataItem
    {
        public virtual int ResourceInventoryLocationID { get; set; }
        public virtual int InventoryLocationID { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
