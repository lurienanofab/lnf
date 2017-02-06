using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Control
{
    public class ControlAction : IDataItem
    {
        public virtual int ActionID { get; set; }
        public virtual string ActionName { get; set; }
        public virtual string ActionTableName { get; set; }
    }
}
