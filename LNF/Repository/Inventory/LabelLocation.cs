using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Repository.Inventory
{
    public class LabelLocation : IDataItem
    {
        public virtual int LabelLocationID { get; set; }
        public virtual LabelRoom LabelRoom { get; set; }
        public virtual string LocationName { get; set; }
    }
}
