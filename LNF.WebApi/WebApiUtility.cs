using System;
using System.Configuration;

namespace LNF.WebApi
{
    public static class WebApiUtility
    {
        public static Uri GetApiHost()
        {
            string apiHost = ConfigurationManager.AppSettings["ApiHost"];

            if (string.IsNullOrEmpty(apiHost))
                throw new InvalidOperationException("Missing appSetting: ApiHost");

            return new Uri(apiHost);
        }
    }
}
