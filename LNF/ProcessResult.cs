using System;
using System.Collections.Generic;

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
        public override string ProcessName { get; }

        public BillingProcessResult(string processName)
        {
            ProcessName = processName;
        }

        public void AddResult<T>(T result) where T : ProcessResult
        {
            AppendResult(result);
        }
    }

    public abstract class DataProcessResult : ProcessResult
    {
        public int RowsDeleted { get; set; }
        public int RowsExtracted { get; set; }
        public int RowsLoaded { get; set; }

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

        public abstract string ProcessName { get; }
        public DateTime StartedAt { get; }
        public DateTime EndedAt => _endedAt ?? throw new Exception("SetEndedAt method must be called first.");
        public IList<string> Data { get; }

        public ProcessLog GetLog()
        {
            StartLog();
            WriteLog();
            WriteLogData();
            return _log;
        }

        public string LogText
        {
            get
            {
                var log = GetLog();
                return GetLogTextRecursive(log, string.Empty);
            }
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

        private DateTime? _endedAt = null;

        public ProcessResult() : this(DateTime.Now, new List<string>()) { }

        private ProcessResult(DateTime startedAt, IList<string> data)
        {
            StartedAt = startedAt;
            Data = data;
        }

        public TimeSpan GetTimeTaken() => EndedAt - StartedAt;

        public void SetEndedAt()
        {
            // Can be used to set _end so that a fixed DateTime is always used to compute time taken.
            // If not called DateTime.Now will be used any time LogText is accessed.
            _endedAt = DateTime.Now;
        }

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
            for (var i = 0; i < Data.Count; ++i)
            {
                _log.Text += $"{Environment.NewLine}[D{i}] {Data[i]}";
            }
        }
    }

}
