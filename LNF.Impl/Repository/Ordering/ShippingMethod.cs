using LNF.DataAccess;

namespace LNF.Impl.Repository.Ordering
{
    public class ShippingMethod : IDataItem
    {
        public virtual int ShippingMethodID { get; set; }
        public virtual string ShippingMethodName { get; set; }
    }
}
