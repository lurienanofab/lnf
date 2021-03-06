﻿using System;

namespace LNF.Billing.Reports
{
    public class BillingSummaryItem : IBillingSummary
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public decimal TotalCharge { get; set; }
        public bool IncludeRemote { get; set; }
    }
}
