using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Repository.Billing
{
    public class ClientOrgBillingTypeLog : IDataItem
    {
        public virtual int ClientOrgBillingTypeLogID { get; set; }
        public virtual ClientOrg ClientOrg { get; set; }
        public virtual BillingType BillingType { get; set; }
        public virtual DateTime EffDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
    }
}
