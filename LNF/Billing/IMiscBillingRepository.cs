using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IMiscBillingRepository
    {
        IMiscBillingCharge GetMiscBillingCharge(int expId);

        IEnumerable<IMiscBillingChargeItem> GetMiscBillingCharges(DateTime period, string[] types, int clientId = 0, int accountId = 0, bool? active = null);

        /// <summary>
        /// Adds a new MiscBillingCharge item to the database. Be sure to recalculate subsidy after doing this.
        /// </summary>
        int CreateMiscBillingCharge(MiscBillingChargeCreateArgs args);

        /// <summary>
        /// Updates a MiscBillingCharge item in the database. Be sure to recalculate subsidy after doing this.
        /// </summary>
        int UpdateMiscBilling(MiscBillingChargeUpdateArgs args);

        /// <summary>
        /// Deletes a MiscBillingCharge item in the database. Be sure to recalculate subsidy after doing this.
        /// </summary>
        int DeleteMiscBillingCharge(int expId);
    }
}
