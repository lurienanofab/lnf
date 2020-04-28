using LNF.Billing;
using System;

namespace LNF.Reporting
{
    public interface IManagerUsageCharge
    {
        string ManagerUsageChargeID { get; set; }
        string UsageChargeID { get; set; }
        DateTime Period { get; set; }
        BillingCategory BillingCategory { get; set; }
        int UserClientID { get; set; }
        string UserUserName { get; set; }
        string UserLName { get; set; }
        string UserFName { get; set; }
        string UserEmail { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        int AccountID { get; set; }
        string ShortCode { get; set; }
        string AccountNumber { get; set; }
        string AccountName { get; set; }
        int OrgID { get; set; }
        string OrgName { get; set; }
        int ChargeTypeID { get; set; }
        double TotalCharge { get; set; }
        double SubsidyDiscount { get; set; }
        bool IsRemote { get; set; }
        bool IsSubsidyOrg { get; set; }
        bool IsMiscCharge { get; set; }
        int ManagerClientID { get; set; }
        string ManagerUserName { get; set; }
        string ManagerLName { get; set; }
        string ManagerFName { get; set; }
        DateTime ManagerEnableDate { get; set; }
        DateTime? ManagerDisableDate { get; set; }
        bool IsTechnicalManager { get; set; }
        bool IsFinancialManager { get; set; }
    }
}
