using System;
using System.Linq;

namespace LNF.Repository.Reporting
{
    public class ManagerUsageCharge : UsageChargeBase
    {
        public virtual int ManagerClientID { get; set; }
        public virtual string ManagerLName { get; set; }
        public virtual string ManagerFName { get; set; }
        public virtual DateTime ManagerEnableDate { get; set; }
        public virtual DateTime? ManagerDisableDate { get; set; }
        public virtual bool IsManager { get; set; }
        public virtual bool IsFinManager { get; set; }

        public static IQueryable<ManagerUsageCharge> SelectByManager(int managerClientId, DateTime period, bool includeRemote)
        {
            return DA.Current.Query<ManagerUsageCharge>().Where(x => x.Period == period && x.ManagerClientID == managerClientId && (!x.IsRemote || includeRemote));
        }
    }
}
