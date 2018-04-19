using System;
using System.Data;
using LNF.Repository;

namespace LNF.CommonTools
{
    public interface IReadRoomDataManager : IManager
    {
        DataTable AggRoomDataClean(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0);
        DataTable NewReadRoomDataRaw(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0);
        DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0);
        DataTable ReadRoomDataClean(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0);
        DataTable ReadRoomDataFiltered(DateTime sDate, DateTime eDate, int clientId = 0, int roomId = 0);
        DataTable ReadRoomDataForUpdate(DateTime TargetDate, out DataSet ds, int clientId = 0, int roomId = 0);
        DataTable ReadRoomDataRaw(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0);
    }
}