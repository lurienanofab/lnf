using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Meter
{
    public class MeterReport : IDataItem
    {
        public virtual int ReportID { get; set; }
        public virtual string ReportName { get; set; }
        public virtual string Header { get; set; }
        public virtual double UnitCost { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
