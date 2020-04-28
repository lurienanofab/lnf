using System;

namespace LNF.Billing
{
    public interface ITieredSubsidyBilling
    {
        int TierBillingID { get; set; }
        DateTime Period { get; set; }
        int ClientID { get; set; }
        int OrgID { get; set; }
        decimal RoomSum { get; set; }
        decimal RoomMiscSum { get; set; }
        decimal ToolSum { get; set; }
        decimal ToolMiscSum { get; set; }
        decimal UserTotalSum { get; set; }
        decimal UserPaymentSum { get; set; }
        DateTime StartingPeriod { get; set; }
        decimal Accumulated { get; set; }
        bool IsNewStudent { get; set; }
        bool IsNewFacultyUser { get; set; }
    }
}
