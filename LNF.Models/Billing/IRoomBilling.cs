using System;

namespace LNF.Models.Billing
{
    public interface IRoomBilling
    {
        decimal AccountDays { get; set; }
        int AccountID { get; set; }
        int BillingTypeID { get; set; }
        decimal ChargeDays { get; set; }
        int ChargeTypeID { get; set; }
        int ClientID { get; set; }
        decimal Entries { get; set; }
        decimal EntryCharge { get; set; }
        decimal EntryRate { get; set; }
        decimal Hours { get; set; }
        bool IsDefault { get; set; }
        bool IsTemp { get; set; }
        decimal MonthlyRoomCharge { get; set; }
        int OrgID { get; set; }
        DateTime Period { get; set; }
        decimal PhysicalDays { get; set; }
        int RoomBillingID { get; set; }
        decimal RoomCharge { get; set; }
        int RoomID { get; set; }
        decimal RoomRate { get; set; }
        decimal SubsidyDiscount { get; set; }

        decimal GetTotalCharge();
    }
}