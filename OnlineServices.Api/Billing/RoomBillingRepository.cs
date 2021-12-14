using LNF.Billing;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class RoomBillingRepository : ApiClient, IRoomBillingRepository
    {
        internal RoomBillingRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<IRoomBilling> CreateRoomBilling(DateTime period, int clientId = 0)
        {
            return Get<List<RoomBillingItem>>("webapi/billing/room/create", QueryStrings(new { period, clientId }));
        }

        public IEnumerable<IRoomData> CreateRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            return Get<List<RoomDataItem>>("webapi/billing/room/data/create", QueryStrings(new { period, clientId, roomId }));
        }

        public IEnumerable<IRoomDataImportLog> GetImportLogs(DateTime sd, DateTime ed)
        {
            return Get<List<RoomDataImportLogItem>>("webapi/billing/room/import-logs", QueryStrings(new { sd, ed }));
        }

        public IEnumerable<IRoomBilling> GetRoomBilling(DateTime period, int clientId = 0, int roomId = 0, int accountId = 0)
        {
            return Get<List<RoomBillingItem>>("webapi/billing/room", QueryStrings(new { period, clientId, roomId, accountId }));
        }

        public IEnumerable<IRoomData> GetRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            return Get<List<RoomDataItem>>("webapi/billing/room/data", QueryStrings(new { period, clientId, roomId }));
        }

        public IEnumerable<IRoomDataClean> GetRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            return Get<List<RoomDataCleanItem>>("webapi/billing/room/data/clean", QueryStrings(new { sd, ed, clientId, roomId }));
        }

        public int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period)
        {
            throw new NotImplementedException();
        }
    }
}
