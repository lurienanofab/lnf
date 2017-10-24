using LNF.PhysicalAccess;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF
{
    public interface IPhysicalAccessProvider : ITypeProvider
    {
        IEnumerable<Badge> GetBadge(Client client = null);
        IEnumerable<Card> GetCards(Badge badge = null);
        IEnumerable<LNF.PhysicalAccess.Area> GetAreas();
        IEnumerable<Badge> CurrentlyInArea();
        IEnumerable<Card> ExpiringCards(DateTime cutoff);
        IEnumerable<Event> GetEvents(DateTime startDate, DateTime endDate, Client client = null, Room room = null);
        bool AllowReenable(int clientId, int dayCount);
        int[] CheckPassbackViolations(DateTime startDate, DateTime endDate);
        void AddClient(Client c);
        void EnableAccess(Client c);
        void EnableAccess(Client c, DateTime expireOn);
        void DisableAccess(Client c);
        void DisableAccess(Client c, DateTime expireOn);
        IEnumerable<RoomDataRaw>  GetRoomData(DateTime sd, DateTime ed, int clientId, string roomName);
    }
}
