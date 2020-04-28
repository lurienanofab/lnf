using LNF.Billing;
using LNF.DataAccess;
using System;
using System.Linq;

namespace LNF.Impl.Repository.Reporting
{
    public class UserUsageCharge : IDataItem
    {
        public virtual string UsageChargeID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual BillingCategory BillingCategory { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string Email { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string AccountName { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual double TotalCharge { get; set; }
        public virtual double SubsidyDiscount { get; set; }
        public virtual bool IsRemote { get; set; }
        public virtual bool IsSubsidyOrg { get; set; }
        public virtual bool IsMiscCharge { get; set; }

        public static IQueryable<UserUsageCharge> SelectByUser(NHibernate.ISession session, int clientId, DateTime period)
        {
            return session.Query<UserUsageCharge>().Where(x => x.Period == period && x.ClientID == clientId);
        }
    }
}
