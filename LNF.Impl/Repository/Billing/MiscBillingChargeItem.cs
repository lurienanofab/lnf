using LNF.Billing;
using LNF.Data;

namespace LNF.Impl.Repository.Billing
{
    public class MiscBillingChargeItem : MiscBillingCharge, IMiscBillingChargeItem
    {
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
    }
}
