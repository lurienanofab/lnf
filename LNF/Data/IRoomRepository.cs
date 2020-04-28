using System.Collections.Generic;

namespace LNF.Data
{
    public interface IRoomRepository
    {
        IEnumerable<IRoom> GetRooms();
        IEnumerable<IRoom> GetActiveRooms(bool? parent = null);
        IEnumerable<IPassbackRoom> GetPassbackRooms();
    }
}
