using LNF.DataAccess;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Repository.Store
{
    public class VendorPackage : IDataItem
    {
        public virtual int VendorPackageID { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual Package Package { get; set; }
        public virtual string VendorPN { get; set; }
        public virtual bool Active { get; set; }
    }
}
