using System;

namespace LNF.Models.Billing
{
    [Flags]
    public enum BillingCategory
    {
        Tool = 1,
        Room = 2,
        Store = 4
    }

    [Flags]
    public enum UpdateDataType
    {
        DataClean = 1,
        Data = 2
    }

    public enum JournalUnitTypes
    {
        All = 0,
        A = 1,
        B = 2,
        C = 3
    }

    public enum ReportTypes
    {
        SUB = 1,
        JU = 2
    }
}
