using System;
using System.Data;
using LNF.Repository;

namespace LNF.CommonTools
{
    public interface IReadRoomDataManager : IManager
    {
        DataSet ReadRoomDataRaw(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0);
        DataSet ReadRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0);
        DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0);
        DataSet ReadRoomDataForUpdate(DateTime period, int clientId = 0, int roomId = 0);
    }
}