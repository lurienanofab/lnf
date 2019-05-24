using LNF.Models.Billing;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class RoomBillingManager : ApiClient, IRoomBillingManager
    {
        public IEnumerable<RoomBillingItem> CreateRoomBilling(DateTime period, int clientId = 0)
        {
            return Get<List<RoomBillingItem>>("webapi/billing/room/create", QueryStrings(new { period, clientId }));
        }

        public IEnumerable<RoomDataItem> CreateRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            return Get<List<RoomDataItem>>("webapi/billing/room/data/create", QueryStrings(new { period, clientId, roomId }));
        }

        public IEnumerable<IRoomDataImportLog> GetImportLogs(DateTime sd, DateTime ed)
        {
            return Get<List<RoomDataImportLogItem>>("webapi/billing/room/import-logs", QueryStrings(new { sd, ed }));
        }

        public IEnumerable<RoomBillingItem> GetRoomBilling(DateTime period, int clientId = 0, int roomId = 0)
        {
            return Get<List<RoomBillingItem>>("webapi/billing/room", QueryStrings(new { period, clientId, roomId }));
        }

        public IEnumerable<RoomDataItem> GetRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            return Get<List<RoomDataItem>>("webapi/billing/room/data", QueryStrings(new { period, clientId, roomId }));
        }

        public IEnumerable<RoomDataCleanItem> GetRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            return Get<List<RoomDataCleanItem>>("webapi/billing/room/data/clean", QueryStrings(new { sd, ed, clientId, roomId }));
        }
    }
}
