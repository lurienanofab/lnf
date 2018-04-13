using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.DataAccess
{
    public static class SessionLog
    {
        private static readonly List<string> _log = new List<string>();

        public static void AddLogMessage(string text, params object[] args)
        {
            _log.Add(string.Format(text, args));
        }

        public static IEnumerable<string> GetLogMessages()
        {
            return _log.AsEnumerable();
        }
    }
}
