using System;

namespace LNF.Billing
{
    public class RoomDataCleanItem : IRoomDataClean
    {
        public int RoomDataID { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public DateTime EntryDT { get; set; }
        public DateTime? ExitDT { get; set; }
        public double Duration { get; set; }
    }
}
