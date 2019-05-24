using System;

namespace LNF.Models.Billing
{
    public class SubLineItem
    {
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int AccountID { get; set; }
    }
}
