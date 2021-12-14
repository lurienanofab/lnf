using System;

namespace LNF.Billing.Apportionment.Models
{
    public class RoomEntryApportionment
    {
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string DisplayName { get; set; }
        public double TotalEntries { get; set; }
    }
}
