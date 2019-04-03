using LNF.Models.Data;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class RoomManager : ApiClient, IRoomManager
    {
        public IEnumerable<IRoom> GetActiveRooms(bool? parent = null)
        {
            if (parent.HasValue)
                return Get<List<RoomItem>>("webapi/data/room/active", QueryStrings(new { parent = parent.Value }));
            else
                return Get<List<RoomItem>>("webapi/data/room/active");
        }
    }
}
