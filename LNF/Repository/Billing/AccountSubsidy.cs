﻿using System;

namespace LNF.Repository.Billing
{
    public class AccountSubsidy : IDataItem
    {
        public virtual int AccountSubsidyID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual decimal UserPaymentPercentage { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime EnableDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
    }
}
