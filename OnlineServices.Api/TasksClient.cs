using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class TasksClient
    {
        public async Task<T> RunJob<T>(Dictionary<string, string> args, T result)
        {
            using (var hc = new HttpClient())
            {
                Uri host = new Uri(ConfigurationManager.AppSettings["ApiHost"]);
                string username = ConfigurationManager.AppSettings["BasicAuthUsername"];
                string password = ConfigurationManager.AppSettings["BasicAuthPassword"];

                var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}"));

                hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                hc.BaseAddress = host;

                FormUrlEncodedContent content = new FormUrlEncodedContent(args);
                var msg = await hc.PostAsync("tasks/api/runjob", content);
                var json = await msg.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeAnonymousType(json, result);
            }
        }
    }
}
