using LNF.Models.Data;
using LNF.Models.PhysicalAccess;
using System;
using System.Collections.Generic;

namespace LNF.Impl.PhysicalAccess
{
    public class PhysicalAccessService : IPhysicalAccessService
    {
        public int AddClient(IClient c)
        {
            throw new NotImplementedException();
        }

        public int DisableAccess(IClient c, DateTime? expireOn = null)
        {
            throw new NotImplementedException();
        }

        public int EnableAccess(IClient c, DateTime? expireOn = null)
        {
            throw new NotImplementedException();
        }

        public bool GetAllowReenable(int clientId, int dayCount)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Area> GetAreas()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Badge> GetBadge(int clientId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetCards(int clientId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Badge> GetCurrentlyInArea(string alias)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetEvents(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetExpiringCards(DateTime cutoff)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetPassbackViolations(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}
