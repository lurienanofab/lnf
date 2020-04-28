using System;

namespace LNF.Billing
{
    public interface IOrgRecharge
    {
        int OrgRechargeID { get; set; }
        int OrgID { get; set; }
        int AccountID { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime EnableDate { get; set; }
        DateTime? DisableDate { get; set; }
    }
}
