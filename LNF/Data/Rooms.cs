using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public enum LabRoom
    {
        TestLab = 2,
        OrganicsBay = 4,
        CleanRoom = 6,
        ChemRoom = 25,
        LNF = 154
    }

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

        public static IRoom TestLab => All().First(x => x.RoomID == (int)LabRoom.TestLab);
        public static IRoom OrganicsBay => All().First(x => x.RoomID == (int)LabRoom.OrganicsBay);
        public static IRoom CleanRoom => All().First(x => x.RoomID == (int)LabRoom.CleanRoom);
        public static IRoom WetChemistry => All().First(x => x.RoomID == (int)LabRoom.ChemRoom);
        public static IRoom LNF => All().First(x => x.RoomID == (int)LabRoom.LNF);

        public static LabRoom GetRoom(int roomId)
        {
            switch (roomId)
            {
                case 2:
                case 4:
                case 6:
                case 25:
                case 154:
                    return (LabRoom)roomId;
                default:
                    throw new Exception($"Unknown RoomID: {roomId}");
            }
        }
    }
}
