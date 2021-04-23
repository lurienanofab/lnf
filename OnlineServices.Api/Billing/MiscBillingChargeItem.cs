using LNF.Billing;
using LNF.Data;

namespace OnlineServices.Api.Billing
{
    public class MiscBillingChargeItem : MiscBillingCharge, IMiscBillingChargeItem
    {
        public string LName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
    }
}
