using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Control
{
    public class ModType : IDataItem
    {
        public virtual int ModTypeID { get; set; }
        public virtual string WagoType { get; set; }
        public virtual int Direction { get; set; }
        public virtual int NumPoints { get; set; }
        public virtual int PointSize { get; set; }
        public virtual int Special { get; set; }
        public virtual string Description { get; set; }
    }
}
