using System;

namespace LNF.Models.Billing
{
    public class RoomDataCleanItem
    {
        public int RoomDataID { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public DateTime EntryDT { get; set; }
        public DateTime? ExitDT { get; set; }
        public double Duration { get; set; }
    }
}
