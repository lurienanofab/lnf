using LNF.Data;
using LNF.PhysicalAccess;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.PhysicalAccess
{
    public class PhysicalAccessService : ApiClient, IPhysicalAccessService
    {
        internal PhysicalAccessService(IRestClient rc) : base(rc) { }

        public IEnumerable<Badge> GetBadge(int clientId = 0)
        {
            return Get<List<Badge>>("webapi/physical-access/badge/{clientId}", UrlSegments(new { clientId }));
        }

        public IEnumerable<Card> GetCards(int clientId = 0)
        {
            return Get<List<Card>>("webapi/physical-access/cards/{clientId}", UrlSegments(new { clientId }));
        }

        public Card GetCard(string cardnum)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetExpiringCards(DateTime cutoff)
        {
            return Get<List<Card>>("webapi/physical-access/cards/expiring", QueryStrings(new { cutoff = cutoff.ToString("yyyy-MM-dd") }));
        }

        public IEnumerable<Area> GetAreas()
        {
            return Get<List<Area>>("webapi/physical-access/areas");
        }

        public IEnumerable<Area> GetAreas(int[] areaIds)
        {
            throw new NotImplementedException();
            //return Post<List<Area>>("webapi/physical-access/areas", areaIds);
        }

        public IEnumerable<Badge> GetCurrentlyInArea(string alias)
        {
            return Get<List<Badge>>("webapi/physical-access/area/{alias}", UrlSegments(new { alias }));
        }

        public IEnumerable<Event> GetEvents(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            return Get<List<Event>>("webapi/physical-access/events", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd"), clientId, roomId }));
        }

        public IEnumerable<Event> GetRawData(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            return Get<List<Event>>("webapi/physical-access/events/raw", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd"), clientId, roomId }));
        }

        public Event FindPreviousIn(FindPreviousInRequest request)
        {
            return Post<Event>("webapi/physical-access/events/find-previous-in", request);
        }

        public Event FindNextOut(FindNextOutRequest request)
        {
            return Post<Event>("webapi/physical-access/events/find-next-out", request);
        }

        public bool GetAllowReenable(int clientId, int days)
        {
            return Get<bool>("webapi/physical-access/allow-reenable", QueryStrings(new { clientId, days }));
        }

        public int[] GetPassbackViolations(DateTime sd, DateTime ed)
        {
            return Get<List<int>>("webapi/physical-access/passback-violations", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") })).ToArray();
        }

        public int AddClient(AddClientRequest request)
        {
            return Post<int>("webapi/physical-access/client/add", request);
        }

        public int EnableAccess(UpdateClientRequest request)
        {
            return Post<int>("webapi/physical-access/client/enable", request);
        }

        public int DisableAccess(UpdateClientRequest request)
        {
            return Post<int>("webapi/physical-access/client/disable", request);
        }

        public IEnumerable<BadgeInArea> GetBadgeInAreas(string alias)
        {
            return Get<List<BadgeInArea>>("webapi/physical-access/badge-in-area", UrlSegments(new { alias }));
        }
    }
}
