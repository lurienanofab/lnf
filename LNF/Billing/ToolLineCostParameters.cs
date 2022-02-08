using System;

namespace LNF.Billing
{
    public class ToolLineCostParameters
    {
        public ToolLineCostParameters() { }

        public ToolLineCostParameters(IToolBilling item)
        {
            Period = item.Period;
            ResourceID = item.ResourceID;
            RoomID = item.RoomID;
            BillingTypeID = item.BillingTypeID;
            IsStarted = item.IsStarted;
            ResourceRate = item.ResourceRate;
            PerUseRate = item.PerUseRate;
            UsageFeeCharged = item.UsageFeeCharged;
            OverTimePenaltyFee = item.OverTimePenaltyFee;
            UncancelledPenaltyFee = item.UncancelledPenaltyFee;
            BookingFee = item.BookingFee;
            ReservationFee2 = item.ReservationFee2;
            IsCancelledBeforeAllowedTime = item.IsCancelledBeforeAllowedTime;
        }

        public DateTime Period { get; set; }
        public int ResourceID { get; set; }
        public int RoomID { get; set; }
        public int BillingTypeID { get; set; }
        public bool IsStarted { get; set; }
        public decimal ResourceRate { get; set; }
        public decimal PerUseRate { get; set; }
        public decimal UsageFeeCharged { get; set; }
        public decimal OverTimePenaltyFee { get; set; }
        public decimal UncancelledPenaltyFee { get; set; }
        public decimal BookingFee { get; set; }
        public decimal ReservationFee2 { get; set; }
        public bool IsCancelledBeforeAllowedTime { get; set; }
    }
}
