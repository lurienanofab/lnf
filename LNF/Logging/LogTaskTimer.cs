using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using LNF.CommonTools;

namespace LNF.Logging
{
    public class LogTaskTimer : IDisposable
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private LogMessageLevel _level;
        private string _subject;
        private string _body;
        private Func<object[]> _args;
        private XElement _data;

        private LogTaskTimer()
        {
            _stopwatch.Start();
        }

        public static LogTaskTimer Start(string subject, string body, Func<object[]> args)
        {
            return Start(LogMessageLevel.Info, subject, body, args);
        }

        public static LogTaskTimer Start(LogMessageLevel logLevel, string subject, string body, Func<object[]> args)
        {
            return new LogTaskTimer()
            {
                _level = logLevel,
                _subject = subject,
                _body = body,
                _args = args,
                _data = XElement.Parse("<root/>")
            };
        }

        public void AddData(string message, params object[] args)
        {
            var node = XElement.Parse("<data/>");
            node.Value = string.Format(message, args);
            _data.Add(node);
        }

        public void Dispose()
        {
            string body = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}: {2}, completed in {3} seconds", DateTime.Now, _subject, string.Format(_body, _args()), _stopwatch.Elapsed.TotalSeconds);
            Log.Write(_level, _subject, body, _data);
        }
    }
}
