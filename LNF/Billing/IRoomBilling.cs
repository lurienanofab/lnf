﻿using System;

namespace LNF.Billing
{
    public interface IRoomBilling
    {
        int RoomBillingID { get; set; }
        DateTime Period { get; set; }
        int ClientID { get; set; }
        int RoomID { get; set; }
        int AccountID { get; set; }
        int ChargeTypeID { get; set; }
        int BillingTypeID { get; set; }
        int OrgID { get; set; }
        decimal ChargeDays { get; set; }
        decimal PhysicalDays { get; set; }
        decimal AccountDays { get; set; }
        decimal Entries { get; set; }
        decimal Hours { get; set; }
        bool IsDefault { get; set; }
        decimal RoomRate { get; set; }
        decimal EntryRate { get; set; }
        decimal MonthlyRoomCharge { get; set; }
        decimal RoomCharge { get; set; }
        decimal EntryCharge { get; set; }
        decimal SubsidyDiscount { get; set; }
        string OrgName { get; set; }
        bool IsTemp { get; }

        /// <summary>
        /// The total charge used to calculate subsidy.
        /// </summary>
        decimal TotalCharge { get; }
    }
}
