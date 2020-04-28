using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class RoomData : IDataItem
    {
        public virtual int RoomDataID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int RoomID { get; set; }
        public virtual int? ParentID { get; set; }
        public virtual bool PassbackRoom { get; set; }
        public virtual DateTime EvtDate { get; set; }
        public virtual int AccountID { get; set; }
        public virtual double Entries { get; set; }
        public virtual double Hours { get; set; }
        public virtual double Days { get; set; }
        public virtual double Months { get; set; }
        public virtual int DataSource { get; set; }
        public virtual bool HasToolUsage { get; set; }
    }
}
