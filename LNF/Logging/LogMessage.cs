using System;
using System.Xml.Linq;

namespace LNF.Logging
{
    public sealed class LogMessage
    {
        public Guid MessageID { get; }
        public DateTime Timestamp { get; }
        public LogMessageLevel Level { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public XElement Data { get; }

        public LogMessage(LogMessageLevel level, string subj, string body)
        {
            MessageID = Guid.NewGuid();
            Timestamp = DateTime.Now;
            Level = level;
            Subject = subj;
            Body = body;
            Data = XElement.Parse("<data/>");
        }

        public void AppendData(string message, params object[] args)
        {
            XElement node = XElement.Parse("<add/>");
            node.Value = string.Format(message, args);
            Data.Add(node);
        }
    }
}
