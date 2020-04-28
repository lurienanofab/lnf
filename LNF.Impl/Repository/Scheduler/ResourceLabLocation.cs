using LNF.DataAccess;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Impl.Repository.Scheduler
{
    public class ResourceLabLocation : IResourceLabLocation, IDataItem
    {
        public virtual int ResourceLabLocationID { get; set; }
        public virtual int LabLocationID { get; set; }
        public virtual int ResourceID { get; set; }
    }
}
