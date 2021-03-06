﻿using System;

namespace LNF.Billing
{
    public class RoomBillingItem : IRoomBilling
    {
        public int RoomBillingID { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public int AccountID { get; set; }
        public int ChargeTypeID { get; set; }
        public int BillingTypeID { get; set; }
        public int OrgID { get; set; }
        public decimal ChargeDays { get; set; }
        public decimal PhysicalDays { get; set; }
        public decimal AccountDays { get; set; }
        public decimal Entries { get; set; }
        public decimal Hours { get; set; }
        public bool IsDefault { get; set; }
        public decimal RoomRate { get; set; }
        public decimal EntryRate { get; set; }
        public decimal MonthlyRoomCharge { get; set; }
        public decimal RoomCharge { get; set; }
        public decimal EntryCharge { get; set; }
        public decimal SubsidyDiscount { get; set; }
        public bool IsTemp { get; set; }
        public string OrgName { get; set; }
        public decimal TotalCharge => GetTotalCharge(RoomCharge, EntryCharge);

        public static decimal GetTotalCharge(decimal roomCharge, decimal entryCharge)
        {
            // this matches the stored procedure TieredSubsidyBilling_Select @Action = 'ForSubsidyDiscountDistribution'
            return roomCharge + entryCharge;
        }
    }
}
