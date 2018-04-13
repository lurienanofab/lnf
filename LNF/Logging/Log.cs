using System;
using System.Linq;
using System.Xml.Linq;

namespace LNF.Logging
{
    public static class Log
    {
        ///<summary>
        ///Attempts to write to an ILogProvider and silently does nothing if the provider is null.
        ///</summary>
        public static void Write(LogMessageLevel level, string subject, string body, XElement data)
        {
            if (ServiceProvider.Current.Log != null)
                ServiceProvider.Current.Log.Current.Write(new LogMessage(level, subject, body, data));
        }

        /// <summary>
        /// Attempts to get all text of current log messages and returns an empty string if the provider is null.
        /// </summary>
        public static string GetText(string separator = null)
        {
            if (separator == null)
                separator = Environment.NewLine;

            if (ServiceProvider.Current.Log != null)
                return string.Join(separator, ServiceProvider.Current.Log.Current.Select(x => x.Body));
            else
                return string.Empty;
        }
    }
}
