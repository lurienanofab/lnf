using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class RoomManager : ManagerBase, IRoomManager
    {
        public RoomManager(IProvider provider) : base(provider){}

        public IEnumerable<IRoom> GetActiveRooms(bool? parent = null)
        {
            IQueryable<Room> query;

            if (parent.HasValue)
            {
                if (parent.Value)
                    query = Session.Query<Room>().Where(x => x.Active && x.ParentID == null);
                else
                    query = Session.Query<Room>().Where(x => x.Active && x.ParentID != null);
            }
            else
            {
                query = Session.Query<Room>().Where(x => x.Active);
            }

            var rooms = query.CreateModels<IRoom>().OrderBy(x => x.RoomDisplayName);

            return rooms;
        }

        public IEnumerable<IPassbackRoom> GetPassbackRooms()
        {
            var dt = Command(CommandType.Text).FillDataTable("SELECT * FROM dbo.v_Area");

            var result = new List<IPassbackRoom>();

            foreach(DataRow dr in dt.Rows)
            {
                result.Add(new PassbackRoomItem
                {
                    AreaID = Convert.ToInt32(dr["AreaID"]),
                    RoomID = Convert.ToInt32(dr["RoomID"]),
                    RoomDisplayName = Convert.ToString(dr["RoomDisplayName"]),
                    AreaName = Convert.ToString(dr["AreaName"])
                });
            }

            return result;
        }
    }
}
