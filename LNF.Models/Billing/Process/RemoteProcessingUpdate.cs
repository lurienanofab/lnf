﻿using System;

namespace LNF.Models.Billing.Process
{
    public class RemoteProcessingUpdate
    {
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public DateTime Period { get; set; }
    }
}
