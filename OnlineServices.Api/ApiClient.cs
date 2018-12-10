using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class ApiClient
    {
        protected IRestClient HttpClient;

        // Using Newtonsoft.Json serializer instead of RestSharp default because it's better (handles enums for example).
        private readonly ISerializer _serializer;

        public ApiClient() : this(GetApiBaseUrl()) { }

        public ApiClient(string host)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            HttpClient = new RestClient(host) { Timeout = 10 * 60 * 1000 };

            var jsonNetSerializer = new JsonNetSerializer();

            // Override with Newtonsoft JSON Handler
            HttpClient.AddHandler("application/json", jsonNetSerializer);
            HttpClient.AddHandler("text/json", jsonNetSerializer);
            HttpClient.AddHandler("text/x-json", jsonNetSerializer);
            HttpClient.AddHandler("text/javascript", jsonNetSerializer);
            HttpClient.AddHandler("*+json", jsonNetSerializer);

            _serializer = jsonNetSerializer;

            string username = ConfigurationManager.AppSettings["BasicAuthUsername"];
            string password = ConfigurationManager.AppSettings["BasicAuthPassword"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                HttpClient.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(username, password);
            }
        }

        protected string Result(IRestResponse resp)
        {
            CheckForError(resp);
            return resp.Content;
        }

        protected T Result<T>(IRestResponse<T> resp)
        {
            CheckForError(resp);
            return resp.Data;
        }

        private void CheckForError(IRestResponse resp)
        {
            if (!resp.IsSuccessful)
            {
                throw new ApiRequestException(resp);
            }
        }

        private void ApplyParameters(IRestRequest req, ParameterCollection parameters)
        {
            if (parameters != null && parameters.Count() > 0)
            {
                foreach (var p in parameters)
                    req.AddParameter(p);
            }
        }

        protected static string GetApiBaseUrl(bool optional = false)
        {
            return GetSetting("ApiBaseUrl", optional);
        }

        protected static string GetSetting(string key, bool optional = false)
        {
            var result = ConfigurationManager.AppSettings[key];

            if (!optional && string.IsNullOrEmpty(result))
                throw new Exception($"Missing required AppSetting: {key}");

            return result;
        }

        protected string Get(string path)
        {
            var req = CreateRestRequest(path, Method.GET);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected string Get(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.GET);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected T Get<T>(string path) where T : new()
        {
            var req = CreateRestRequest(path, Method.GET);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Get<T>(string path, ParameterCollection parameters) where T : new()
        {
            var req = CreateRestRequest(path, Method.GET);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected async Task<string> GetAsync(string path)
        {
            var req = CreateRestRequest(path, Method.GET);
            var resp = await HttpClient.ExecuteTaskAsync(req);
            return Result(resp);
        }

        protected async Task<string> GetAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.GET);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync(req);
            return Result(resp);
        }

        protected async Task<T> GetAsync<T>(string path)
        {
            var req = CreateRestRequest(path, Method.GET);
            var resp = await HttpClient.ExecuteTaskAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> GetAsync<T>(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.GET);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync<T>(req);
            return Result(resp);
        }

        protected string Post(string path)
        {
            var req = CreateRestRequest(path, Method.POST);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected string Post(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected string Post(string path, object model)
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected string Post(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected T Post<T>(string path) where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Post<T>(string path, ParameterCollection parameters) where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Post<T>(string path, object model) where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Post<T>(string path, object model, ParameterCollection parameters) where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path)
        {
            var req = CreateRestRequest(path, Method.POST);
            var resp = await HttpClient.ExecuteTaskAsync(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path, object model)
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            var resp = await HttpClient.ExecuteTaskAsync(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path)
        {
            var req = CreateRestRequest(path, Method.POST);
            var resp = await HttpClient.ExecuteTaskAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path, object model)
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            var resp = await HttpClient.ExecuteTaskAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            req.AddJsonBody(model);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync<T>(req);
            return Result(resp);
        }

        protected bool Put(string path, object model)
        {
            var req = CreateRestRequest(path, Method.PUT);
            req.AddJsonBody(model);
            var resp = HttpClient.Execute<bool>(req);
            return Result(resp);
        }

        protected bool Put(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<bool>(req);
            return Result(resp);
        }

        protected bool Put(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            req.AddJsonBody(model);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<bool>(req);
            return Result(resp);
        }

        protected async Task<bool> PutAsync(string path, object model)
        {
            var req = CreateRestRequest(path, Method.PUT);
            req.AddJsonBody(model);
            var resp = await HttpClient.ExecuteTaskAsync<bool>(req);
            return Result(resp);
        }

        protected async Task<bool> PutAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync<bool>(req);
            return Result(resp);
        }

        protected async Task<bool> PutAsync(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            req.AddJsonBody(model);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync<bool>(req);
            return Result(resp);
        }

        protected int Delete(string path)
        {
            var req = CreateRestRequest(path, Method.DELETE);
            var resp = HttpClient.Execute<int>(req);
            return Result(resp);
        }

        protected int Delete(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.DELETE);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<int>(req);
            return Result(resp);
        }

        protected async Task<int> DeleteAsync(string path)
        {
            var req = CreateRestRequest(path, Method.DELETE);
            var resp = await HttpClient.ExecuteTaskAsync<int>(req);
            return Result(resp);
        }

        protected async Task<int> DeleteAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.DELETE);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteTaskAsync<int>(req);
            return Result(resp);
        }

        protected ParameterCollection QueryStrings(object parameters)
        {
            return Parameters(parameters, ParameterType.QueryString, new CreateParameterOptions { UseLowerCaseForEnumValues = false });
        }

        protected ParameterCollection UrlSegments(object parameters)
        {
            return Parameters(parameters, ParameterType.UrlSegment, new CreateParameterOptions() { SkipNull = false });
        }

        protected ParameterCollection Parameters(object parameters, ParameterType type, CreateParameterOptions opts)
        {
            var list = new List<Parameter>();

            var props = parameters.GetType().GetProperties();

            foreach (var p in props)
            {
                var name = p.Name;
                var value = p.GetValue(parameters);
                if (!opts.SkipNull || value != null)
                    list.Add(CreateParameter(name, value, type, opts));
            }

            var result = new ParameterCollection { list };

            return result;
        }

        protected IRestRequest CreateRestRequest(string path, Method method)
        {
            return new RestRequest(path, method) { JsonSerializer = _serializer };
        }

        protected Parameter CreateParameter(string name, object value, ParameterType type)
        {
            // use default options (lcase enum values)
            return CreateParameter(name, value, type, new CreateParameterOptions());
        }

        protected Parameter CreateParameter(string name, object value, ParameterType type, CreateParameterOptions opts)
        {
            if (opts == null)
                throw new ArgumentNullException("opts");

            var result = new Parameter
            {
                Name = name,
                Type = type
            };

            if (value is DateTime)
            {
                var d = Convert.ToDateTime(value);
                if (d.TimeOfDay.TotalSeconds == 0)
                    result.Value = d.ToString(opts.DateFormat);
                else
                    result.Value = d.ToString(opts.DateTimeFormat);
            }
            else if (value is Enum)
            {
                if (opts.UseLowerCaseForEnumValues)
                    result.Value = value.ToString().ToLower();
                else
                    result.Value = value.ToString();
            }
            else
            {
                result.Value = value;
            }

            return result;
        }
    }

    public class ParameterCollection : IEnumerable<Parameter>
    {
        private List<Parameter> _items = new List<Parameter>();

        public ParameterCollection() { }

        public static ParameterCollection operator &(ParameterCollection v1, ParameterCollection v2)
        {
            var result = new ParameterCollection { v1, v2 };
            return result;
        }

        public void Add(IEnumerable<Parameter> items)
        {
            _items.AddRange(items);
        }

        public void Add(Parameter item)
        {
            _items.Add(item);
        }

        public void Add(string name, object value, ParameterType type)
        {
            _items.Add(new Parameter(name, value, type));
        }

        public int Count => _items.Count;

        public IEnumerator<Parameter> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class CreateParameterOptions
    {
        public bool UseLowerCaseForEnumValues { get; set; } = true;
        public bool SkipNull { get; set; } = true;
        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:ss";
    }
}
