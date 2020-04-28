using LNF.DataAccess;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Repository.Store
{
    public class Package : IDataItem
    {
        public virtual int PackageID { get; set; }
        public virtual Item Item { get; set; }
        public virtual decimal BaseQMultiplier { get; set; }
        public virtual string Descriptor { get; set; }
        public virtual bool Active { get; set; }
    }
}
