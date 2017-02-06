using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class DemGender : IDataItem
    {
        public virtual int DemGenderID { get; set; }
        public virtual string DemGenderValue { get; set; }
    }
}
