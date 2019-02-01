using System;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IMiscClient
    {
        MiscBillingChargeItem GetMiscBillingCharge(int expId);
        IEnumerable<MiscBillingChargeItem> GetMiscBillingCharges(DateTime period, int? clientId = null, bool? active = null);
        int CreateMiscBillingCharge(MiscBillingChargeCreateArgs args);
        int UpdateMiscBilling(MiscBillingChargeUpdateArgs args);
        int DeleteMiscBillingCharge(int expId);
    }
}
