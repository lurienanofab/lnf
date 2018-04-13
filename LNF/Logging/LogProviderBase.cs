using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Logging
{
    public abstract class LogProviderBase : ILogService
    {
        private LogServiceElement _config;

        public LogProviderBase()
        {
            _config = ServiceProvider.GetConfigurationSection().Log;
        }

        public virtual ILog Current
        {
            get
            {
                ILog result = ServiceProvider.Current.Context.GetItem<ILog>("current_log");
                if (result == null)
                {
                    result = NewLog();
                    ServiceProvider.Current.Context.SetItem("current_log", result);
                }
                return result;
            }
        }

        public string Name => _config.Name;
        public bool Enabled => _config.Enabled;

        protected abstract LogBase NewLog();
    }

    public abstract class LogBase : ILog
    {
        private List<Exception> exceptions = new List<Exception>();
        private List<LogMessage> messages = new List<LogMessage>();

        protected abstract void OnWrite(LogMessage message);
        protected abstract int OnPurge(DateTime cutoff);

        public void Write(LogMessage message)
        {
            if (ServiceProvider.Current.Log.Enabled)
            {
                messages.Add(message);
                OnWrite(message);
            }
        }

        public int Purge(DateTime cutoff)
        {
            LogMessage[] removed = messages.Where(x => x.Timestamp < cutoff).ToArray();
            foreach (var item in removed)
                messages.Remove(item);
            return OnPurge(cutoff);
        }

        public virtual string GetText()
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogMessage msg in messages)
                sb.AppendLine(string.Format("{0}: {1}", Enum.GetName(typeof(LogMessageLevel), msg.Level).ToUpper(), msg.Body));
            return sb.ToString();
        }

        public virtual string GetHtml()
        {
            return GetText().Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");
        }

        public void AddException(Exception ex)
        {
            exceptions.Add(ex);
        }

        public IEnumerable<Exception> GetExceptions()
        {
            return exceptions.AsEnumerable();
        }

        public IEnumerator<LogMessage> GetEnumerator()
        {
            return messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
