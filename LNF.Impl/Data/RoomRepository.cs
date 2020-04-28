using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class RoomRepository : RepositoryBase, IRoomRepository
    {
        public RoomRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IRoom> GetRooms()
        {
            return Session.Query<Room>().CreateModels<IRoom>();
        }

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
            var table = Session.CreateSQLQuery("SELECT * FROM dbo.v_Area").FillTable();

            var result = new List<IPassbackRoom>();

            foreach(IDictionary row in table)
            {
                result.Add(new PassbackRoomItem
                {
                    AreaID = Convert.ToInt32(row["AreaID"]),
                    RoomID = Convert.ToInt32(row["RoomID"]),
                    RoomDisplayName = Convert.ToString(row["RoomDisplayName"]),
                    AreaName = Convert.ToString(row["AreaName"])
                });
            }

            return result;
        }
    }
}
