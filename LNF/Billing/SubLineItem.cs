using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Billing
{
    public class SubLineItem
    {
        public virtual DateTime Period { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int AccountID { get; set; }
    }
}
