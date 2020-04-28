using LNF.DataAccess;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Repository.Store
{
    public class StoreOrderDetail : IDataItem
    {
        public virtual int SODID { get; set; }
        public virtual StoreOrder StoreOrder { get; set; }
        public virtual Item Item { get; set; }
        public virtual int Quantity { get; set; }
        public virtual int PriceID { get; set; }
    }
}
