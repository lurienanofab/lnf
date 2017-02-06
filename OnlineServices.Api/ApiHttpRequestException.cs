using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineServices.Api
{
    public class ApiHttpRequestException : HttpRequestException
    {
        private string _Message;
        private string _StackTrace;

        public HttpStatusCode StatusCode { get; }
        public override string StackTrace { get { return _StackTrace; } }
        public override string Message { get { return string.Format("[{0} {1}] {2}", (int)StatusCode, StatusCode, _Message).Trim(); } }

        public string GetRawMessage()
        {
            return _Message;
        }

        private ApiHttpRequestException(HttpStatusCode statusCode, string message = null, string stackTrace = null)
        {
            StatusCode = statusCode;
            _StackTrace = stackTrace;
            _Message = message;
        }

        public static async Task<ApiHttpRequestException> Create(HttpResponseMessage msg)
        {
            if (msg.StatusCode == HttpStatusCode.NotFound)
            {
                //in this case the content will be HTML
                return new ApiHttpRequestException(msg.StatusCode, msg.RequestMessage.RequestUri.ToString());
            }

            var content = await msg.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeAnonymousType(content, new { Message = "", ExceptionMessage = "", ExceptionType = "", StackTrace = "" });
            if (error != null)
            {
                //prefer to use ExceptionMessage when possible
                string errmsg = string.IsNullOrEmpty(error.ExceptionMessage) ? error.Message : error.ExceptionMessage;
                return new ApiHttpRequestException(msg.StatusCode, errmsg, error.StackTrace);
            }
            else
                return new ApiHttpRequestException(msg.StatusCode);
        }
    }
}
