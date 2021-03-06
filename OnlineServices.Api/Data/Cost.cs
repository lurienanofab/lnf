﻿using System;
using LNF.Data;

namespace OnlineServices.Api.Data
{
    public class Cost : ICost
    {
        public int CostID { get; set; }
        public int ChargeTypeID { get; set; }
        public string TableNameOrDescription { get; set; }
        public int? RecordID { get; set; }
        public string AcctPer { get; set; }
        public decimal AddVal { get; set; }
        public decimal MulVal { get; set; }
        public DateTime EffDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
