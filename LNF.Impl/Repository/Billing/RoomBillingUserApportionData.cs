using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class RoomBillingUserApportionData : IDataItem
    {
        public virtual int RoomBillingUserApportionDataID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual Client Client { get; set; }
        public virtual Room Room { get; set; }
        public virtual Account Account { get; set; }
        public virtual decimal ChargeDays { get; set; }
        public virtual decimal Entries { get; set; }
    }
}
