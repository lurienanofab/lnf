using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Control
{
    public class Point : IDataItem
    {
        public virtual int PointID { get; set; }
        public virtual Block Block { get; set; }
        public virtual int ModPosition { get; set; }
        public virtual int Offset { get; set; }
        public virtual string Name { get; set; }
    }
}
