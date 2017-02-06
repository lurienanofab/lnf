using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class FeedsLog : IDataItem
    {
        public virtual int FeedsLogID { get; set; }
        public virtual DateTime EntryDateTime { get; set; }
        public virtual string RequestIP { get; set; }
        public virtual string RequestURL { get; set; }
        public virtual string RequestUserAgent { get; set; }
    }
}
