using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Control
{
    public class BlockConfig : IDataItem
    {
        public virtual int ConfigID { get; set; }
        public virtual Block Block { get; set; }
        public virtual ModType ModType { get; set; }
        public virtual byte ModPosition { get; set; }
    }
}
