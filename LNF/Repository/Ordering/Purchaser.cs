using LNF.Repository.Data;

namespace LNF.Repository.Ordering
{
    public class Purchaser : IDataItem
    {
        public virtual int PurchaserID { get; set; }
        public virtual Client Client { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
