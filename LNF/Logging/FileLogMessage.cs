using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace LNF.Logging
{
    public abstract class FileLogMessage<T> : LogMessage<T>
    {
        protected abstract string FileExtension { get; }

        protected FileLogMessage(string name, T message) : base(name, message) { }

        public string GetLogFilePath()
        {
            var dir = ConfigurationManager.AppSettings["LogDirectory"];

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return Path.Combine(dir, Name + FileExtension);
        }
    }
}
