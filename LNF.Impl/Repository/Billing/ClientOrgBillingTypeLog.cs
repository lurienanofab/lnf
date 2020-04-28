using LNF.Billing;
using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class ClientOrgBillingTypeLog : IClientOrgBillingTypeLog, IDataItem
    {
        public virtual int ClientOrgBillingTypeLogID { get; set; }
        public virtual int ClientOrgID { get; set; }
        public virtual int BillingTypeID { get; set; }
        public virtual DateTime EffDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
    }
}
