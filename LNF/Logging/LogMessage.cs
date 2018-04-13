using System;
using System.Xml.Linq;

namespace LNF.Logging
{
    public sealed class LogMessage
    {
        public LogMessage()
        {
            Timestamp = DateTime.Now;
            Data = XElement.Parse("<root/>");
        }

        public LogMessage(LogMessageLevel level, string subject, string body, XElement data)
            : this()
        {
            Level = level;
            Subject = subject;
            Body = body;
            Data = data; //replace
        }

        public Guid MessageID
        {
            get
            {
                Guid result = ServiceProvider.Current.Context.GetItem<Guid>("LogMessageID");
                if (result == null)
                    result = Guid.NewGuid();
                return result;
            }
        }

        public DateTime Timestamp { get; }
        public LogMessageLevel Level { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public XElement Data { get; set; }

        public void AppendData(string message, params object[] args)
        {
            XElement node = XElement.Parse("<data/>");
            node.Value = string.Format(message, args);
            Data.Add(node);
        }
    }

    public abstract class LogMessage<T> : ILogMessage
    {
        public string Name { get; protected set; }
        public T Message { get; protected set; }
        public abstract void Write();

        protected LogMessage(string name, T message)
        {
            Name = name;
            Message = message;
        }

        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
