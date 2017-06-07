using LNF.Cache;
using LNF.CommonTools;
using LNF.Repository.Data;
using System.Linq;
using System;

namespace LNF.Data
{
    public static class RoomUtility
    {
        public static Rooms GetRoom(int roomId)
        {
            return (Rooms)roomId;
        }

        public static string GetRoomDisplayName(int roomId)
        {
            var room = CacheManager.Current.Rooms().FirstOrDefault(x => x.RoomID == roomId);

            if (room == null)
                return string.Format("[unknown:{0}]", roomId);
            else
                return room.RoomDisplayName;
        }
    }
}
