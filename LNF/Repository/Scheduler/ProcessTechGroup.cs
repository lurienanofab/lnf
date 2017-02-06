using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Repository.Scheduler
{
    public class ProcessTechGroup : IDataItem
    {
        public virtual int ProcessTechGroupID { get; set; }
        public virtual string GroupName { get; set; }
    }
}
