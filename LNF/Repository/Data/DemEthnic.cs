using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class DemEthnic : IDataItem
    {
        public virtual int DemEthnicID { get; set; }
        public virtual string DemEthnicValue { get; set; }
    }
}
