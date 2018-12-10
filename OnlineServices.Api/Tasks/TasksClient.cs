using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace OnlineServices.Api.Tasks
{
    public class TasksClient : ApiClient
    {
        public TasksClient() : base(GetApiBaseUrl()) { }

        public T RunJob<T>(Dictionary<string, string> args, T result)
        {
            var req = CreateRestRequest("tasks/api/runjob", Method.POST);

            foreach(var kvp in args)
                req.AddParameter(kvp.Key, kvp.Value);

            var resp = HttpClient.Execute(req);

            var json = resp.Content;

            return JsonConvert.DeserializeAnonymousType(json, result);
        }
    }
}
