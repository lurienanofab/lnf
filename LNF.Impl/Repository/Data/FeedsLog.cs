using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
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
