using System;

namespace LNF.Data
{
    public interface IFeedsLog
    {
        DateTime EntryDateTime { get; set; }
        int FeedsLogID { get; set; }
        string RequestIP { get; set; }
        string RequestURL { get; set; }
        string RequestUserAgent { get; set; }
    }
}