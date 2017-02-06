using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Scheduler
{
    public class ProcessInfoLineParam : IDataItem
    {
        public virtual int ProcessInfoLineParamID { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual string ParameterName { get; set; }
        public virtual int ParameterType { get; set; }
    }
}
