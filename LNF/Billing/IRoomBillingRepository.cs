using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IRoomBillingRepository
    {
        IEnumerable<IRoomBilling> CreateRoomBilling(DateTime period, int clientId = 0);
        IEnumerable<IRoomData> CreateRoomData(DateTime period, int clientId = 0, int roomId = 0);
        IEnumerable<IRoomBilling> GetRoomBilling(DateTime period, int clientId = 0, int roomId = 0, int accountId = 0);
        IEnumerable<IRoomData> GetRoomData(DateTime period, int clientId = 0, int roomId = 0);
        IEnumerable<IRoomDataClean> GetRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0);
        IEnumerable<IRoomDataImportLog> GetImportLogs(DateTime sd, DateTime ed);
        int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period);
    }
}