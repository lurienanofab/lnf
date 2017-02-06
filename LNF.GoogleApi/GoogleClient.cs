using LNF.Repository.Data;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LNF.GoogleApi
{
    public class GoogleClient : IDisposable
    {
        public GoogleAuthorization Authorization { get; private set; }

        private readonly HttpClient httpClient;

        public GoogleClient(GoogleAuthorization authorization)
        {
            Authorization = authorization;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.googleapis.com");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Authorization.AccessToken);
        }

        public async Task<UserInfo> GetUserInfo()
        {
            var msg = await httpClient.GetAsync("/plus/v1/people/me");

            var content = await msg.Content.ReadAsStringAsync();

            if (!msg.IsSuccessStatusCode)
                throw new HttpException((int)msg.StatusCode, content);

            var result = JsonConvert.DeserializeObject<UserInfo>(content);

            return result;
        }

        public async Task<DriveFile[]> GetFiles()
        {
            var msg = await httpClient.GetAsync("/drive/v2/files");

            var content = await msg.Content.ReadAsStringAsync();

            if (!msg.IsSuccessStatusCode)
                throw new HttpException((int)msg.StatusCode, content);

            var fileList = JsonConvert.DeserializeObject<Google.Apis.Drive.v2.Data.FileList>(content);

            var result = fileList.Items.Select(x=>new DriveFile(x)).ToArray();

            return result;
        }

        public async Task<string> CreateSpreadsheet(DataFeed feed)
        {
            string title = string.Format("{0}-{1:yyyyMMddHHmmss}", feed.FeedAlias, DateTime.Now);
            string mimeType = "application/vnd.google-apps.spreadsheet";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/upload/drive/v2/files?uploadType=media");

            request.Content = new StringContent(JsonConvert.SerializeObject(new { title, mimeType }), Encoding.UTF8, mimeType);

            var msg = await httpClient.SendAsync(request);

            //HttpContent body = new StringContent(JsonConvert.SerializeObject(new { title, mimeType }));
            //body.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            //var msg = await httpClient.PostAsync("/upload/drive/v2/files?uploadType=media", body);

            var result = await msg.Content.ReadAsStringAsync();

            if (msg.IsSuccessStatusCode)
                return result;
            else
                throw new HttpException((int)msg.StatusCode, result);
        }

        public async Task<string> CreateWorksheet(DataFeed feed)
        {
            string payload = "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:gs=\"http://schemas.google.com/spreadsheets/2006\">"
                + string.Format("<title>{0}-{1:yyyyMMddHHmmss}</title>", feed.FeedAlias, DateTime.Now)
                + "<gs:rowCount>50</gs:rowCount>"
                + "<gs:colCount>10</gs:colCount>"
                + "</entry>";

            HttpContent body = new StringContent(payload, Encoding.UTF8, "application/atom+xml");

            var msg = await httpClient.PostAsync("/feeds/worksheets/key/private/full", body);

            var result = await msg.Content.ReadAsStringAsync();

            if (msg.IsSuccessStatusCode)
                return result;
            else
                throw new HttpException((int)msg.StatusCode, result);
        }

        public void Dispose()
        {
            if (httpClient != null)
                httpClient.Dispose();
        }
    }
}