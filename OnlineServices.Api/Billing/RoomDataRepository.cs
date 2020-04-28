using LNF.Billing;
using System;
using System.Data;

namespace OnlineServices.Api.Billing
{
    public class RoomDataRepository : ApiClient, IRoomDataRepository
    {
        public IRoomData AddRoomData(IRoomDataClean rdc)
        {
            throw new NotImplementedException();
        }

        public IRoomDataClean AddRoomDataClean(int clientId, int roomId, DateTime entryDT, DateTime exitDT, double duration)
        {
            throw new NotImplementedException();
        }

        public string GetEmail(IRoomData item)
        {
            throw new NotImplementedException();
        }

        public DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            throw new NotImplementedException();
        }

        public DataSet ReadRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            throw new NotImplementedException();
        }

        public DataSet ReadRoomDataForUpdate(DateTime period, int clientId = 0, int roomId = 0)
        {
            throw new NotImplementedException();
        }

        public DataSet ReadRoomDataRaw(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
