using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface IFeedRepository
    {
        IScriptEngine ScriptEngine { get; }
        DataFeedResult GetDataFeedResult(string alias, string key = null, IDictionary<object, object> parameters = null);
        IDataFeed GetDataFeed(int feedId);
        IDataFeed GetDataFeed(string alias);
        IFeedsLog AddFeedsLogEntry(string requestIp, string requestUrl, string userAgent);
        IEnumerable<IReservationFeed> GetReservationFeeds(DateTime sd, DateTime ed, int resourceId = 0);
        IEnumerable<IReservationFeed> GetReservationFeeds(string username, DateTime sd, DateTime ed, int resourceId = 0);
    }
}
