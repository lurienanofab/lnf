using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class Rooms
    {
        public static IEnumerable<IRoom> All() => CacheManager.Current.GetValue("Rooms", p => p.Data.Room.GetRooms(), DateTimeOffset.Now.AddDays(7));

        public static string GetRoomDisplayName(int roomId)
        {
            var room = All().FirstOrDefault(x => x.RoomID == roomId);

            if (room == null)
                return string.Format("[unknown:{0}]", roomId);
            else
                return room.RoomDisplayName;
        }

        public static IRoom TestLab => All().First(x => x.RoomID == 2);
        public static IRoom OrganicsBay => All().First(x => x.RoomID == 4);
        public static IRoom CleanRoom => All().First(x => x.RoomID == 6);
        public static IRoom WetChemistry => All().First(x => x.RoomID == 25);
        public static IRoom LNF => All().First(x => x.RoomID == 154);
    }
}
