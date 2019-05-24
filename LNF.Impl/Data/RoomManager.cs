using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
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
    }
}
