using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Repository.Billing
{
    public class RoomDataWithToolUsage : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int RoomID { get; set; }
        public virtual double TotalEntriesPerMonth { get; set; }
        public virtual double TotalHoursPerMonth { get; set; }
        public virtual int PhysicalDays { get; set; }

        public override bool Equals(object obj)
        {
            RoomDataWithToolUsage x = obj as RoomDataWithToolUsage;
            
            if (obj == null) return false;

            return x.Period == Period && x.ClientID == ClientID && x.RoomID == RoomID;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}", Period, ClientID, RoomID).GetHashCode();
        }
    }
}
