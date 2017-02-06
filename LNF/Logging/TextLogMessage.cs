using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LNF.Logging
{
    public class TextLogMessage : FileLogMessage<string>
    {
        protected TextLogMessage(string name, string message) : base(name, message) { }

        public static TextLogMessage Create(string name, string message, params object[] args)
        {
            return new TextLogMessage(name, string.Format(message, args));
        }

        protected override string FileExtension
        {
            get { return ".log"; }
        }

        public override void Write()
        {
            var path = GetLogFilePath();
            File.AppendAllLines(path, new[] { string.Format("[{0}] {1}", GetCurrentTime(), Message) });
        }
    }
}
