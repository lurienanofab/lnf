using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class Area : IDataItem
    {
        public virtual int AreaID { get; set; }
        public virtual string AreaName { get; set; }
    }
}
