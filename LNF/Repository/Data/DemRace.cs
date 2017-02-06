using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class DemRace : IDataItem
    {
        public virtual int DemRaceID { get; set; }
        public virtual string DemRaceValue { get; set; }
    }
}
