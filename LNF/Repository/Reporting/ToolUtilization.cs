
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Reporting
{
    public class ToolUtilization : IDataItem
    {
        public virtual DateTime Period { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int ProcessTechID { get; set; }
        public virtual string ProcessTechName { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual double TotalActDurationHours { get; set; }
        public virtual double TotalChargeDurationHours { get; set; }
        public virtual double TotalChargeDurationForgivenHours { get; set; }
        public virtual double TotalTransferredDurationHours { get; set; }
        public virtual double TotalSchedDurationHours { get; set; }
        public virtual double TotalOverTimeHours { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ToolUtilization item = obj as ToolUtilization;
            if (item == null) return false;
            return item.Period == this.Period
                && item.ResourceID == this.ResourceID
                && item.ActivityID == this.ActivityID;
        }

        public override int GetHashCode()
        {
            return (this.Period.ToString("yyyy-MM-dd HH:mm:ss") + "|"
                + this.ResourceID.ToString() + "|"
                + this.ActivityID.ToString()).GetHashCode();
        }
    }
}
