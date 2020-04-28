using LNF.Impl.DataAccess;
using LNF.Logging;
using System;
using System.Configuration;
using System.IO;

namespace LNF.Impl.Logging
{
    public abstract class FileLoggingService : LoggingServiceBase
    {
        public FileLoggingService(ISessionManager mgr) : base(mgr) { }

        /// <summary>
        /// The log file extension (should start with '.')
        /// </summary>
        protected abstract string FileExtension { get; }

        protected string GetLogFilePath()
        {
            var dir = ConfigurationManager.AppSettings["LogDirectory"];

            if (string.IsNullOrWhiteSpace(dir))
                throw new Exception("Missing required appSetting: LogDirectory");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return Path.Combine(dir, Name + FileExtension);
        }

        protected override void AddMessage(LogMessage message)
        {
            File.AppendAllText(GetLogFilePath(), FormatMessage(message));
        }
    }
}
