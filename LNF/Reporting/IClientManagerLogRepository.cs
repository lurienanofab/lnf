using System;
using System.Collections.Generic;

namespace LNF.Reporting
{
    public interface IClientManagerLogRepository
    {
        /// <summary>
        /// Selects all manager/client account relationships that were active during the specified date range.
        /// </summary>
        IEnumerable<IClientManagerLog> SelectByManager(int clientId, DateTime sd, DateTime ed);

        IEnumerable<IClientManagerLog> SelectByPeriod(DateTime sd, DateTime ed);
    }
}
