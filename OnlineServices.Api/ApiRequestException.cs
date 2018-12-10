using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace OnlineServices.Api
{
    public class ApiRequestException : Exception
    {
        private readonly string _Message;
        private readonly string _StackTrace;

        public HttpStatusCode StatusCode { get; }
        public override string StackTrace { get { return _StackTrace; } }
        public override string Message { get { return string.Format("[{0} {1}] {2}", (int)StatusCode, StatusCode, _Message).Trim(); } }

        public string GetRawMessage()
        {
            return _Message;
        }

        public ApiRequestException(IRestResponse resp)
        {
            StatusCode = resp.StatusCode;

            if (StatusCode == HttpStatusCode.NotFound)
            {
                //in this case the content will be HTML
                _Message = resp.Request.Resource;
                _StackTrace = null;
                return;
            }

            if (resp.ContentType.StartsWith("text/html"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(resp.Content, "<title>(.+)</title>");
                if (match.Success && match.Groups.Count > 1)
                    _Message = match.Groups[1].Value;
            }
            else if (resp.ContentType.StartsWith("application/json"))
            {
                var error = JsonConvert.DeserializeAnonymousType(resp.Content, new { Message = "", ExceptionMessage = "", ExceptionType = "", StackTrace = "" });

                //prefer to use ExceptionMessage when possible
                string errmsg = string.IsNullOrEmpty(error.ExceptionMessage) ? error.Message : error.ExceptionMessage;
                _Message = errmsg;
                _StackTrace = error.StackTrace;
            }
            else
            {
                _Message = null;
                _StackTrace = null;
            }
        }
    }
}
