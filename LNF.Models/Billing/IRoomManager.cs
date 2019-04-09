using System;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IRoomManager
    {
        IEnumerable<RoomBillingItem> CreateRoomBilling(DateTime period, int clientId = 0);
        IEnumerable<RoomDataItem> CreateRoomData(DateTime period, int clientId = 0, int roomId = 0);
        IEnumerable<RoomBillingItem> GetRoomBilling(DateTime period, int clientId = 0, int roomId = 0);
        IEnumerable<RoomDataItem> GetRoomData(DateTime period, int clientId = 0, int roomId = 0);
        IEnumerable<RoomDataCleanItem> GetRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0);
    }
}