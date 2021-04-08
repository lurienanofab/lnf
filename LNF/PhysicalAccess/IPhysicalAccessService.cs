using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.PhysicalAccess
{
    public interface IPhysicalAccessService
    {
        IEnumerable<Badge> GetBadge(int clientId = 0);
        IEnumerable<Card> GetCards(int clientId = 0);
        Card GetCard(string cardnum);
        IEnumerable<Area> GetAreas();
        IEnumerable<Area> GetAreas(int[] areaIds);
        IEnumerable<Badge> GetCurrentlyInArea(string alias);
        IEnumerable<Card> GetExpiringCards(DateTime cutoff);
        IEnumerable<Event> GetEvents(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0);
        bool GetAllowReenable(int clientId, int days);
        int[] GetPassbackViolations(DateTime sd, DateTime ed);
        int AddClient(IClient c);
        int EnableAccess(IClient c, DateTime? expireOn = null);
        int DisableAccess(IClient c, DateTime? expireOn = null);
        IEnumerable<Event> GetRawData(DateTime sd, DateTime ed, int clientId, int roomId);
        Event FindPreviousIn(Event e, DateTime sd);
        Event FindNextOut(Event e, DateTime ed);
    }
}
