using System;

namespace LNF.Billing
{
    public interface IOrgRecharge
    {
        int OrgRechargeID { get; set; }
        int OrgID { get; set; }
        string OrgName { get; set; }
        bool OrgActive { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
        bool AccountActive { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime EnableDate { get; set; }
        DateTime? DisableDate { get; set; }
    }
}
