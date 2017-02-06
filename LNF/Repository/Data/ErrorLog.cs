using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class ErrorLog : IDataItem
    {
        public virtual int ErrorLogID { get; set; }
        public virtual string Application { get; set; }
        public virtual string Message { get; set; }
        public virtual string StackTrace { get; set; }
        public virtual DateTime ErrorDateTime { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string PageUrl { get; set; }
    }
}
