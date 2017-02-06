using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Repository.Scheduler
{
    public class SchedulerProperty : IDataItem
    {
        public virtual int PropertyID { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string PropertyValue { get; set; }
    }
}
