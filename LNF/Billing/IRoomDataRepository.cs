using System;
using System.Data;

namespace LNF.Billing
{
    public interface IRoomDataRepository
    {
        DataSet ReadRoomDataRaw(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0);
        DataSet ReadRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0);
        DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0);
        DataSet ReadRoomDataForUpdate(DateTime period, int clientId = 0, int roomId = 0);
        IRoomDataClean AddRoomDataClean(int clientId, int roomId, DateTime entryDT, DateTime exitDT, double duration);
        IRoomData AddRoomData(IRoomDataClean rdc);
        string GetEmail(IRoomData item);
    }
}