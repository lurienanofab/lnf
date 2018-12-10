using System;
using System.Configuration;

namespace LNF.WebApi
{
    public static class WebApiUtility
    {
        public static Uri GetApiBaseUrl()
        {
            string host = ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrEmpty(host))
                throw new InvalidOperationException("Missing appSetting: ApiBaseUrl");

            return new Uri(host);
        }
    }
}
