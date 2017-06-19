using System;

namespace LNF.PhysicalAccess
{
    public class RoomDataRaw
    {
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
    }
}
