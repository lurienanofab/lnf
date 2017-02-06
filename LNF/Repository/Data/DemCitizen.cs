using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class DemCitizen : IDataItem
    {
        public virtual int DemCitizenID { get; set; }
        public virtual string DemCitizenValue { get; set; }
    }
}
