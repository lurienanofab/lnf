using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Billing
{
    public class ApportionmentClient : IDataItem
    {
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Emails { get; set; }
        public virtual int AccountCount { get; set; }
    }
}
