using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF
{
    public class ProcessLog
    {
        private readonly IList<ProcessLog> _logs;

        public DateTime Time { get; }
        public string Text { get; set; }
        public IEnumerable<ProcessLog> Logs => _logs;

        public ProcessLog()
        {
            _logs = new List<ProcessLog>();
            Time = DateTime.Now;
        }

        public void AddChild<T>(T result) where T : ProcessResult
        {
            if (result != null)
            {
                var log = result.GetLog();
                if (log != null)
                    _logs.Add(log);
                else
                    _logs.Add(new ProcessLog() { Text = $"{typeof(T).Name} log is null" });
            }
            else
            {
                _logs.Add(new ProcessLog() { Text = $"{typeof(T).Name} is null" });
            }
        }

        public override string ToString() => Text;
    }

    public class BillingProcessResult : DataProcessResult
    {
        protected BillingProcessResult() { }

        public BillingProcessResult(string processName, DateTime startedAt, IEnumerable<ProcessResult> results = null) : base(startedAt)
        {
            ProcessName = processName;

            if (results != null)
            {
                foreach (var res in results)
                    AddResult(res);
            }
        }

        public override string ProcessName { get; }

        public void AddResult<T>(T result) where T : ProcessResult
        {
            AppendResult(result);
        }
    }

    public abstract class DataProcessResult : ProcessResult
    {
        protected DataProcessResult() { }

        public DataProcessResult(DateTime startedAt) : base(startedAt, null) { }

        public virtual int RowsDeleted { get; set; }
        public virtual int RowsExtracted { get; set; }
        public virtual int RowsLoaded { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"RowsDeleted: {RowsDeleted}");
            AppendLog($"RowsExtracted: {RowsExtracted}");
            AppendLog($"RowsLoaded: {RowsLoaded}");
        }
    }

    public abstract class ProcessResult
    {
        private readonly ProcessLog _log = new ProcessLog();
        private IList<string> _data;

        protected ProcessResult() { }

        public ProcessResult(DateTime startedAt, IEnumerable<string> data)
        {
            StartedAt = startedAt;
            EndedAt = DateTime.Now;
            Data = data;
        }

        public abstract string ProcessName { get; }
        public virtual DateTime StartedAt { get; protected set; }
        public virtual DateTime EndedAt { get; protected set; }

        public virtual IEnumerable<string> Data
        {
            get
            {
                if (_data == null)
                    _data = new List<string>();
                return _data;
            }
            protected set
            {
                if (value == null)
                    _data = new List<string>();
                else
                    _data = value.ToList();
            }
        }

        public string LogText
        {
            get
            {
                var log = GetLog();
                var logText = GetLogTextRecursive(log, string.Empty);
                return logText;
            }
        }

        public ProcessLog GetLog()
        {
            StartLog();
            WriteLog();
            WriteLogData();
            return _log;
        }

        private string GetLogTextRecursive(ProcessLog log, string indent)
        {
            string result = indent + log.Text;
            foreach (var l in log.Logs)
            {
                result += Environment.NewLine + GetLogTextRecursive(l, indent + "  ");
            }
            return result;
        }

        public TimeSpan GetTimeTaken() => EndedAt - StartedAt;

        private void StartLog()
        {
            var timeTaken = GetTimeTaken();
            _log.Text = $"{ProcessName} process completed at {EndedAt:yyyy-MM-dd HH:mm:ss} (time taken: {timeTaken.TotalSeconds:0.0000}).";
        }

        protected void AppendLog(string text)
        {
            string separator = _log.Text.EndsWith(".") ? " " : ", ";
            _log.Text += separator + text;
        }

        protected void AppendResult<T>(T result) where T : ProcessResult
        {
            _log.AddChild(result);
        }

        protected virtual void WriteLog()
        {
            // Does nothing unless overridden by derived class.
        }

        private void WriteLogData()
        {
            for (var i = 0; i < _data.Count; ++i)
            {
                _log.Text += $"{Environment.NewLine}[D{i}] {_data[i]}";
            }
        }
    }

}