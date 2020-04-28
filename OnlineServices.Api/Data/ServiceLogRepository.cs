using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineServices.Api.Data
{
    public class ServiceLogRepository : ApiClient, IServiceLogRepository
    {
        public IEnumerable<IServiceLog> GetServiceLogs(int limit, int skip = 0, Guid? id = null, string service = null, string subject = null)
        {
            string url = "webapi/data/servicelog";

            ParameterCollection parameters = new ParameterCollection();

            parameters.Add("limit", limit, ParameterType.QueryString);

            if (skip > 0)
            {
                parameters.Add("skip", skip, ParameterType.QueryString);
            }

            if (id.HasValue)
            {
                url += "/{id}";
                parameters.Add("id", id.ToString().ToLower(), ParameterType.UrlSegment);
            }

            if (!string.IsNullOrEmpty(service))
            {
                parameters.Add("service", service, ParameterType.QueryString);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                parameters.Add("subject", subject, ParameterType.QueryString);
            }

            return Get<List<ServiceLogItem>>(url, parameters);
        }

        public void InsertServiceLog(IServiceLog model)
        {
            var item = Post<ServiceLogItem>("webapi/data/servicelog", model);
            model.ServiceLogID = item.ServiceLogID;
        }

        public bool UpdateServiceLog(Guid id, string data)
        {
            IDictionary<string, string> postData = new Dictionary<string, string> { { "", data } };
            ParameterCollection parameters = new ParameterCollection { postData.Select(x => new Parameter(x.Key, x.Value, ParameterType.RequestBody)) };
            return Put(string.Format("webapi/data/servicelog/{0}", id), parameters);
        }
    }
}
