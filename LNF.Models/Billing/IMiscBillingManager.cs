using System;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IMiscBillingManager
    {
        IMiscBillingCharge GetMiscBillingCharge(int expId);

        IEnumerable<IMiscBillingCharge> GetMiscBillingCharges(DateTime period, int? clientId = null, bool? active = null);

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
