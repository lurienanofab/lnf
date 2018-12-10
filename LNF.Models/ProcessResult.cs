using System;
using System.Collections.Generic;

namespace LNF.Models
{
    public class ProcessResult
    {
        public string ProcessName { get; }
        public DateTime Start { get; }
        public IList<string> Data { get; }
        public int RowsDeleted { get; set; }
        public int RowsExtracted { get; set; }
        public int RowsLoaded { get; set; }

        private DateTime? _end = null;
        private string _logText = string.Empty;

        public ProcessResult(string processName) : this(processName, DateTime.Now, new List<string>()) { }

        private ProcessResult(string processName, DateTime startedAt, IList<string> data)
        {
            ProcessName = processName;
            Start = startedAt;
            Data = data;
        }

        public string LogText
        {
            get
            {
                StartLog();
                WriteLog();
                WriteLogData();
                return _logText;
            }
        }

        public TimeSpan GetTimeTaken()
        {
            if (!_end.HasValue)
                _end = DateTime.Now;

            return _end.Value - Start;
        }

        private void StartLog()
        {
            var timeTaken = GetTimeTaken();
            _logText = $"{ProcessName} process completed at {_end.Value:yyyy-MM-dd HH:mm:ss} (time taken: {timeTaken}).";
        }

        protected void AppendLog(string text)
        {
            _logText += $" {text}.";
        }

        protected virtual void WriteLog()
        {
            AppendLog($"RowsDeleted: {RowsDeleted}");
            AppendLog($"RowsExtracted: {RowsExtracted}");
            AppendLog($"RowsLoaded: {RowsLoaded}");
        }

        private string WriteLogData()
        {
            string result = string.Empty;
            if (Data.Count > 0)
                result = $"{Environment.NewLine}{Environment.NewLine}----- Data -------------------{Environment.NewLine}{string.Join(Environment.NewLine, Data)}.";
            return result;
        }
    }

}
