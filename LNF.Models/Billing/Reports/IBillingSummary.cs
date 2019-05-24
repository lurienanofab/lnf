using System;

namespace LNF.Models.Billing.Reports
{
    public interface IBillingSummary
    {
        int ChargeTypeID { get; set; }
        string ChargeTypeName { get; set; }
        int ClientID { get; set; }
        DateTime EndDate { get; set; }
        bool IncludeRemote { get; set; }
        DateTime StartDate { get; set; }
        decimal TotalCharge { get; set; }
    }
}