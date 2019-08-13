using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IRoomManager
    {
        IEnumerable<IRoom> GetActiveRooms(bool? parent = null);
        IEnumerable<IPassbackRoom> GetPassbackRooms();
    }
}
