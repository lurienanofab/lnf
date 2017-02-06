using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;

namespace LNF.Repository.Store
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
