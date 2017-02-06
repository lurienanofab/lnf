using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Billing
{
    public class ClientAccountBillingType : IDataItem
    {
        public virtual int LogID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int ClientOrgID { get; set; }
        public virtual int ClientAccountID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual int OrgID { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual int ClientOrgBillingTypeID { get; set; }
        public virtual int BillingTypeID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string Email { get; set; }
        public virtual string OrgName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string Name { get; set; }
    }
}
