using System;

namespace LNF.Data
{
    public class FeedsLogItem : IFeedsLog
    {
        public int FeedsLogID { get; set; }
        public DateTime EntryDateTime { get; set; }
        public string RequestIP { get; set; }
        public string RequestURL { get; set; }
        public string RequestUserAgent { get; set; }
    }
}
