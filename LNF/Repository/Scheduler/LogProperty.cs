using System;
using System.Text;
using System.Collections.Generic;

namespace LNF.Repository.Scheduler
{
    public class LogProperty : IDataItem
    {
        public virtual int LogPropertyID { get; set; }
        public virtual string PropertyName{ get; set; }
    }
}