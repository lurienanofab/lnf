﻿using LNF;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class ApiClient
    {
        protected IRestClient HttpClient;

        // Using Newtonsoft.Json serializer instead of RestSharp default because it's better (handles enums for example).

        internal ApiClient(IRestClient rc)
        {
            HttpClient = rc;
        }

        private ApiClient() : this(GetApiBaseUrl()) { }

        private ApiClient(string host) : this(NewRestClient(host)) { }

        public static string GetApiBaseUrl(bool optional = false)
        {
            return GetSetting("ApiBaseUrl", optional);
        }

        public static IRestClient NewRestClient()
        {
            return NewRestClient(GetApiBaseUrl());
        }

        public static IRestClient NewRestClient(string host)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            var rc = new RestClient(host) { Timeout = 10 * 60 * 1000 };

            // Override with Newtonsoft JSON Handler
            rc.AddHandler("application/json", () => JsonNetDeserializer.Default);
            rc.AddHandler("text/json", () => JsonNetDeserializer.Default);
            rc.AddHandler("text/x-json", () => JsonNetDeserializer.Default);
            rc.AddHandler("text/javascript", () => JsonNetDeserializer.Default);
            rc.AddHandler("*+json", () => JsonNetDeserializer.Default);

            string username = ConfigurationManager.AppSettings["BasicAuthUsername"];
            string password = ConfigurationManager.AppSettings["BasicAuthPassword"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                rc.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(username, password);
            }

            return rc;
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
                    req.AddParameter(p.Name, p.Value, p.Type);
            }
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

        protected T Get<T>(string path) //where T : new()
        {
            var req = CreateRestRequest(path, Method.GET);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Get<T>(string path, ParameterCollection parameters) //where T : new()
        {
            var req = CreateRestRequest(path, Method.GET);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected async Task<string> GetAsync(string path)
        {
            var req = CreateRestRequest(path, Method.GET);
            var resp = await HttpClient.ExecuteAsync<string>(req);
            return Result(resp);
        }

        protected async Task<string> GetAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.GET);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<string>(req);
            return Result(resp);
        }

        protected async Task<T> GetAsync<T>(string path)
        {
            var req = CreateRestRequest(path, Method.GET);
            var resp = await HttpClient.ExecuteAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> GetAsync<T>(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.GET);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<T>(req);
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
            SetJsonContent(req, model);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected string Post(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            SetJsonContent(req, model);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected T Post<T>(string path) //where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Post<T>(string path, ParameterCollection parameters) //where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Post<T>(string path, object model) //where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            SetJsonContent(req, model);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected string Post(string path, FileCollection files)
        {
            var req = CreateRestRequest(path, Method.POST);

            if (files.Count > 0)
            {
                foreach (var f in files)
                    req.AddFile(files.Name, f.Data, f.FileName);
            }

            var resp = HttpClient.Execute(req);
            return Result(resp);
        }

        protected T Post<T>(string path, FileCollection files) //where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);

            if (files.Count > 0)
            {
                foreach (var f in files)
                    req.AddFile(files.Name, f.Data, f.FileName);
            }

            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Post<T>(string path, object model, ParameterCollection parameters) //where T : new()
        {
            var req = CreateRestRequest(path, Method.POST);
            SetJsonContent(req, model);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path)
        {
            var req = CreateRestRequest(path, Method.POST);
            var resp = await HttpClient.ExecuteAsync<string>(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<string>(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path, object model)
        {
            var req = CreateRestRequest(path, Method.POST);
            SetJsonContent(req, model);
            var resp = await HttpClient.ExecuteAsync<string>(req);
            return Result(resp);
        }

        protected async Task<string> PostAsync(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            SetJsonContent(req, model);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<string>(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path)
        {
            var req = CreateRestRequest(path, Method.POST);
            var resp = await HttpClient.ExecuteAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path, object model)
        {
            var req = CreateRestRequest(path, Method.POST);
            SetJsonContent(req, model);
            var resp = await HttpClient.ExecuteAsync<T>(req);
            return Result(resp);
        }

        protected async Task<T> PostAsync<T>(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.POST);
            SetJsonContent(req, model);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<T>(req);
            return Result(resp);
        }

        protected bool Put(string path, object model)
        {
            var req = CreateRestRequest(path, Method.PUT);
            SetJsonContent(req, model);
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
            SetJsonContent(req, model);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<bool>(req);
            return Result(resp);
        }

        protected T Put<T>(string path, object model)
        {
            var req = CreateRestRequest(path, Method.PUT);
            SetJsonContent(req, model);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Put<T>(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected T Put<T>(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            SetJsonContent(req, model);
            ApplyParameters(req, parameters);
            var resp = HttpClient.Execute<T>(req);
            return Result(resp);
        }

        protected async Task<bool> PutAsync(string path, object model)
        {
            var req = CreateRestRequest(path, Method.PUT);
            SetJsonContent(req, model);
            var resp = await HttpClient.ExecuteAsync<bool>(req);
            return Result(resp);
        }

        protected async Task<bool> PutAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<bool>(req);
            return Result(resp);
        }

        protected async Task<bool> PutAsync(string path, object model, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.PUT);
            SetJsonContent(req, model);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<bool>(req);
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
            var resp = await HttpClient.ExecuteAsync<int>(req);
            return Result(resp);
        }

        protected async Task<int> DeleteAsync(string path, ParameterCollection parameters)
        {
            var req = CreateRestRequest(path, Method.DELETE);
            ApplyParameters(req, parameters);
            var resp = await HttpClient.ExecuteAsync<int>(req);
            return Result(resp);
        }

        protected ParameterCollection QueryStrings(IDictionary<object, object> dict)
        {
            return Parameters(dict, ParameterType.QueryString, new CreateParameterOptions { UseLowerCaseForEnumValues = false });
        }

        protected ParameterCollection QueryStrings(object parameters)
        {
            return Parameters(parameters, ParameterType.QueryString, new CreateParameterOptions { UseLowerCaseForEnumValues = false });
        }

        protected ParameterCollection UrlSegments(IDictionary<object, object> dict)
        {
            return Parameters(dict, ParameterType.UrlSegment, new CreateParameterOptions { SkipNull = false });
        }

        protected ParameterCollection UrlSegments(object parameters)
        {
            return Parameters(parameters, ParameterType.UrlSegment, new CreateParameterOptions { SkipNull = false });
        }

        protected ParameterCollection Parameters(object parameters, ParameterType type, CreateParameterOptions opts)
        {
            var dict = new Dictionary<object, object>();

            var props = parameters.GetType().GetProperties();

            foreach (var p in props)
            {
                var name = p.Name;
                var value = p.GetValue(parameters);
                dict.Add(name, value);
            }

            return Parameters(dict, type, opts);
        }

        protected ParameterCollection Parameters(IDictionary<object, object> dict, ParameterType type, CreateParameterOptions opts)
        {
            var list = new List<ParameterItem>();

            foreach (var kvp in dict)
            {
                var name = kvp.Key.ToString();
                var value = kvp.Value;
                if (!opts.SkipNull || value != null)
                {
                    list.Add(CreateParameter(name, value, type, opts));
                }
            }

            var result = new ParameterCollection { list };

            return result;
        }

        protected IRestRequest CreateRestRequest(string path, Method method)
        {
            return new RestRequest(path, method);
        }

        protected void SetJsonContent(IRestRequest req, object body)
        {
            req.JsonSerializer = JsonNetSerializer.Default;
            req.AddJsonBody(body);
        }

        protected ParameterItem CreateParameter(string name, object value, ParameterType type)
        {
            // use default options (lcase enum values)
            return CreateParameter(name, value, type, new CreateParameterOptions());
        }

        protected ParameterItem CreateParameter(string name, object value, ParameterType type, CreateParameterOptions opts)
        {
            if (opts == null)
                throw new ArgumentNullException("opts");

            object val;

            if (value is DateTime)
            {
                var d = Convert.ToDateTime(value);
                if (d.TimeOfDay.TotalSeconds == 0)
                    val = d.ToString(opts.DateFormat);
                else
                    val = d.ToString(opts.DateTimeFormat);
            }
            else if (value is Enum)
            {
                if (opts.UseLowerCaseForEnumValues)
                    val = value.ToString().ToLower();
                else
                    val = value.ToString();
            }
            else
            {
                val = value;
            }

            var result = new ParameterItem(name, val, type);

            return result;
        }
    }

    public class ParameterItem
    {
        public string Name { get; }
        public object Value { get; }
        public ParameterType Type { get; }

        public ParameterItem(string name, object value, ParameterType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }

    public class ParameterCollection : IEnumerable<ParameterItem>
    {
        private readonly List<ParameterItem> _items = new List<ParameterItem>();

        public ParameterCollection() { }

        public static ParameterCollection operator &(ParameterCollection v1, ParameterCollection v2)
        {
            var result = new ParameterCollection { v1, v2 };
            return result;
        }

        public void Add(IEnumerable<ParameterItem> items)
        {
            _items.AddRange(items);
        }

        public void Add(ParameterItem item)
        {
            _items.Add(item);
        }

        public void Add(string name, object value, ParameterType type)
        {
            _items.Add(new ParameterItem(name, value, type));
        }

        public int Count => _items.Count;

        public IEnumerator<ParameterItem> GetEnumerator()
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
