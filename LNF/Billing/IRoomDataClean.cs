using System;

namespace LNF.Billing
{
    public interface IRoomDataClean
    {
        int ClientID { get; set; }
        double Duration { get; set; }
        DateTime EntryDT { get; set; }
        DateTime? ExitDT { get; set; }
        int RoomDataID { get; set; }
        int RoomID { get; set; }
    }
}