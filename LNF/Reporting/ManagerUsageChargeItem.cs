using LNF.Billing;
using System;

namespace LNF.Reporting
{
    public class ManagerUsageChargeItem
    {
        public string ManagerUsageChargeID { get; set; }
        public string UsageChargeID { get; set; }
        public DateTime Period { get; set; }
        public BillingCategory BillingCategory { get; set; }
        public int UserClientID { get; set; }
        public string UserUserName { get; set; }
        public string UserLName { get; set; }
        public string UserFName { get; set; }
        public string UserEmail { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int AccountID { get; set; }
        public string ShortCode { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public int ChargeTypeID { get; set; }
        public double TotalCharge { get; set; }
        public double SubsidyDiscount { get; set; }
        public bool IsRemote { get; set; }
        public bool IsSubsidyOrg { get; set; }
        public bool IsMiscCharge { get; set; }
        public int ManagerClientID { get; set; }
        public string ManagerUserName { get; set; }
        public string ManagerLName { get; set; }
        public string ManagerFName { get; set; }
        public DateTime ManagerEnableDate { get; set; }
        public DateTime? ManagerDisableDate { get; set; }
        public bool IsTechnicalManager { get; set; }
        public bool IsFinancialManager { get; set; }
    }
}
