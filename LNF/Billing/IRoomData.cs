using System;

namespace LNF.Billing
{
    public interface IRoomData
    {
        int AccountID { get; set; }
        int ClientID { get; set; }
        int DataSource { get; set; }
        double Days { get; set; }
        double Entries { get; set; }
        DateTime EvtDate { get; set; }
        bool HasToolUsage { get; set; }
        double Hours { get; set; }
        double Months { get; set; }
        int? ParentID { get; set; }
        bool PassbackRoom { get; set; }
        DateTime Period { get; set; }
        int RoomDataID { get; set; }
        int RoomID { get; set; }
    }
}