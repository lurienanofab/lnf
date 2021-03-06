using LNF.Billing;
using LNF.DataAccess;
using LNF.Reporting;
using System;

namespace LNF.Impl.Repository.Reporting
{
    public class ManagerUsageCharge : IManagerUsageCharge, IDataItem
    {
        public virtual string ManagerUsageChargeID { get; set; }
        public virtual string UsageChargeID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual BillingCategory BillingCategory { get; set; }
        public virtual int UserClientID { get; set; }
        public virtual string UserUserName { get; set; }
        public virtual string UserLName { get; set; }
        public virtual string UserFName { get; set; }
        public virtual string UserEmail { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string AccountName { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual double TotalCharge { get; set; }
        public virtual double SubsidyDiscount { get; set; }
        public virtual bool IsRemote { get; set; }
        public virtual bool IsSubsidyOrg { get; set; }
        public virtual bool IsMiscCharge { get; set; }
        public virtual int ManagerClientID { get; set; }
        public virtual string ManagerUserName { get; set; }
        public virtual string ManagerLName { get; set; }
        public virtual string ManagerFName { get; set; }
        public virtual DateTime ManagerEnableDate { get; set; }
        public virtual DateTime? ManagerDisableDate { get; set; }
        public virtual bool IsTechnicalManager { get; set; }
        public virtual bool IsFinancialManager { get; set; }
    }
}
