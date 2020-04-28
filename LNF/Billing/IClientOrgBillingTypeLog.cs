using System;

namespace LNF.Billing
{
    public interface IClientOrgBillingTypeLog
    {
        int ClientOrgBillingTypeLogID { get; set; }
        int ClientOrgID { get; set; }
        int BillingTypeID { get; set; }
        DateTime EffDate { get; set; }
        DateTime? DisableDate { get; set; }
    }
}
