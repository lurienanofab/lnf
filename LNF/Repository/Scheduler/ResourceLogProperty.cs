using System;
using System.Text;
using System.Collections.Generic;

namespace LNF.Repository.Scheduler
{
    public class ResourceLogProperty : IDataItem
    {
        public virtual int ResourceLogPropertyID { get; set; }
        public virtual LogProperty LogProperty{ get; set; }
        public virtual Resource Resource { get; set; }
        public virtual string PropertyType { get; set; }
    }
}