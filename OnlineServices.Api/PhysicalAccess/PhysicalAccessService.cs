using LNF.Models;
using LNF.Models.Data;
using LNF.Models.PhysicalAccess;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.PhysicalAccess
{
    public class PhysicalAccessService : ApiClient, IPhysicalAccessService
    {
        public PhysicalAccessService() : base(GetApiBaseUrl()) { }

        public IEnumerable<Badge> GetBadge(int clientId = 0)
        {
            return Get<List<Badge>>("webapi/physical-access/badge/{clientId}", UrlSegments(new { clientId }));
        }

        public IEnumerable<Card> GetCards(int clientId = 0)
        {
            return Get<List<Card>>("webapi/physical-access/cards/{clientId}", UrlSegments(new { clientId }));
        }

        public IEnumerable<Card> GetExpiringCards(DateTime cutoff)
        {
            return Get<List<Card>>("webapi/physical-access/cards/expiring", QueryStrings(new { cutoff = cutoff.ToString("yyyy-MM-dd") }));
        }

        public IEnumerable<Area> GetAreas()
        {
            return Get<List<Area>>("webapi/physical-access/areas");
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

        public Event FindPreviousIn(Event e, DateTime sd)
        {
            return Post<Event>("webapi/physical-access/events/find-previous-in", e, QueryStrings(new { sd = sd.ToString("yyyy-MM-dd") }));
        }

        public Event FindNextOut(Event e, DateTime ed)
        {
            return Post<Event>("webapi/physical-access/events/find-next-out", e, QueryStrings(new { ed = ed.ToString("yyyy-MM-dd") }));
        }

        public bool GetAllowReenable(int clientId, int days)
        {
            return Get<bool>("webapi/physical-access/allow-reenable", QueryStrings(new { clientId, days }));
        }

        public int[] GetPassbackViolations(DateTime sd, DateTime ed)
        {
            return Get<List<int>>("webapi/physical-access/passback-violations", QueryStrings(new { sd = sd.ToString("yyyy-MM-dd"), ed = ed.ToString("yyyy-MM-dd") })).ToArray();
        }

        public int AddClient(IClient c)
        {
            return Post<int>("webapi/physical-access/client/add", c);
        }

        public int EnableAccess(IClient c, DateTime? expireOn = null)
        {
            ParameterCollection parameters = null;

            if (expireOn.HasValue)
                parameters = new ParameterCollection { new Parameter[] { new Parameter("expireOn", expireOn.Value.ToString("yyyy-MM-dd"), ParameterType.QueryString) } };

            return Post<int>("webapi/physical-access/client/enable", c, parameters);
        }

        public int DisableAccess(IClient c, DateTime? expireOn = null)
        {
            ParameterCollection parameters = null;

            if (expireOn.HasValue)
                parameters = new ParameterCollection { new Parameter("expireOn", expireOn.Value.ToString("yyyy-MM-dd"), ParameterType.QueryString) };

            return Post<int>("webapi/physical-access/client/disable", c, parameters);
        }
    }
}
