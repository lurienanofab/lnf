using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IRoomDataManager : IManager
    {
        RoomData Create(RoomDataClean rdc);
        string GetEmail(RoomData item);
    }
}