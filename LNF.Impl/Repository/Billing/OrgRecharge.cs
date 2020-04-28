using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class OrgRecharge : IDataItem
    {
        public virtual int OrgRechargeID { get; set; }
        public virtual Org Org { get; set; }
        public virtual Account Account { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime EnableDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
    }
}
