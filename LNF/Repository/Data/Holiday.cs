using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class Holiday : IDataItem
    {
        public virtual int HolidayID { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime HolidayDate { get; set; }
    }
}
