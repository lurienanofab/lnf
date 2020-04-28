using LNF.DataAccess;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Repository.Ordering
{
    public class Purchaser : IDataItem
    {
        public virtual int PurchaserID { get; set; }
        public virtual Client Client { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
