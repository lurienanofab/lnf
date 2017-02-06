using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Control
{
    public class Property : IDataItem
    {
        public virtual int PropertyID { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string PropertyValue { get; set; }
    }
}
