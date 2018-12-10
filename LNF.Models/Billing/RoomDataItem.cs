using System;

namespace LNF.Models.Billing
{
    public class RoomDataItem
    {
        public int RoomDataID { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public int? ParentID { get; set; }
        public bool PassbackRoom { get; set; }
        public DateTime EvtDate { get; set; }
        public int AccountID { get; set; }
        public double Entries { get; set; }
        public double Hours { get; set; }
        public double Days { get; set; }
        public double Months { get; set; }
        public int DataSource { get; set; }
        public bool HasToolUsage { get; set; }
    }
}
