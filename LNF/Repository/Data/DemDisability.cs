using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class DemDisability : IDataItem
    {
        public virtual int DemDisabilityID { get; set; }
        public virtual string DemDisabilityValue { get; set; }
    }
}
