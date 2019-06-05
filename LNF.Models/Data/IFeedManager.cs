using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IFeedManager
    {
        DataFeedResult GetDataFeedResult(string alias, string key = null, IDictionary<object, object> parameters = null);
    }
}
