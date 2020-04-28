using System;

namespace LNF.Billing
{
    public class TieredSubsidyBillingItem : ITieredSubsidyBilling
    {
        public int TierBillingID { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int OrgID { get; set; }
        public decimal RoomSum { get; set; }
        public decimal RoomMiscSum { get; set; }
        public decimal ToolSum { get; set; }
        public decimal ToolMiscSum { get; set; }
        public decimal UserTotalSum { get; set; }
        public decimal UserPaymentSum { get; set; }
        public DateTime StartingPeriod { get; set; }
        public decimal Accumulated { get; set; }
        public bool IsNewStudent { get; set; }
        public bool IsNewFacultyUser { get; set; }
    }
}
