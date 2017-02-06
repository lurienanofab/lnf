using System;

namespace LNF.Repository.Data
{
    public class RoomData : IDataItem
    {
        public virtual int RoomDataID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Room Room { get; set; }
        public virtual DateTime EvtDate { get; set; }
        public virtual Account Account { get; set; }
        public virtual decimal Entries { get; set; }
        public virtual decimal Hours { get; set; }
        public virtual decimal Days { get; set; }
        public virtual decimal Months { get; set; }
        public virtual int DataSource { get; set; }
        public virtual bool HasToolUsage { get; set; }
    }
}
