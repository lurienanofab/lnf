using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Control;

namespace LNF.Repository.Control
{
    public class ActionInstance : IDataItem, IControlInstance
    {
        public virtual int Index { get; set; }
        public virtual int Point { get; set; }
        public virtual int ActionID { get; set; }
        public virtual string Name { get; set; }
        public virtual string ActionName { get; set; }
    }
}
