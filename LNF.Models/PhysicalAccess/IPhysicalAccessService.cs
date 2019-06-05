using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace LNF.Models.PhysicalAccess
{
    public interface IPhysicalAccessService
    {
        IEnumerable<Badge> GetBadge(int clientId = 0);
        IEnumerable<Card> GetCards(int clientId = 0);
        IEnumerable<Area> GetAreas();
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
