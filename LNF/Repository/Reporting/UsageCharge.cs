using LNF.Models.Billing;
using System;
using System.Linq;

namespace LNF.Repository.Reporting
{
    public class UsageCharge : IDataItem
    {
        public virtual string UsageChargeID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual BillingCategory BillingCategory { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string OrgName { get; set; }
        public virtual double TotalCharge { get; set; }
        public virtual double SubsidyDiscount { get; set; }
        public virtual bool IsRemote { get; set; }
        public virtual bool IsSubsidyOrg { get; set; }
        public virtual int ManagerClientID { get; set; }
        public virtual string ManagerLName { get; set; }
        public virtual string ManagerFName { get; set; }
        public virtual DateTime ManagerEnableDate { get; set; }
        public virtual DateTime? ManagerDisableDate { get; set; }
        public virtual bool IsManager { get; set; }
        public virtual bool IsFinManager { get; set; }
        public virtual bool MiscCharge { get; set; }

        public static IQueryable<UsageCharge> SelectByManager(DateTime period, int managerClientId, bool includeRemote)
        {
            return DA.Current.Query<UsageCharge>().Where(x => x.Period == period && x.ManagerClientID == managerClientId && (!x.IsRemote || includeRemote));
        }
    }
}
