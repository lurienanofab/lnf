using LNF.Control;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Control
{
    public class BlockConfig : IBlockConfig, IDataItem
    {
        public virtual int ConfigID { get; set; }
        public virtual int BlockID { get; set; }
        public virtual int ModTypeID { get; set; }
        public virtual byte ModPosition { get; set; }
    }
}
