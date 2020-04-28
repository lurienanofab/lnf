using System;
using System.Collections.Generic;

namespace LNF.Reporting
{
    public interface IManagerUsageChargeRepository
    {
        IEnumerable<IManagerUsageCharge> GetManagerUsageCharges(DateTime sd, DateTime ed, bool remote = false);

        IEnumerable<IManagerUsageCharge> GetManagerUsageCharges(int clientId, DateTime sd, DateTime ed, bool remote = false);

        /// <summary>
        /// Returns charges incurred by users of a specified manager.
        /// </summary>
        /// <param name="clientId">The manager's ClientID</param>
        /// <param name="period">The period.</param>
        /// <param name="includeRemote">Indicates if remote processing charges are included.</param>
        IEnumerable<IManagerUsageCharge> SelectByManager(int clientId, DateTime period, bool includeRemote);

        IEnumerable<IManagerUsageCharge> SelectByPeriod(DateTime period, bool includeRemote);
    }
}
