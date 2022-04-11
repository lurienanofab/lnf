using LNF.CommonTools;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LNF.Impl.DataAccess
{
    public static class SessionLog
    {
        private static readonly List<string> _log = new List<string>();

        public static bool Enabled => Utility.GetAppSetting("SessionLogEnabled") == "true";

        public static void AddLogMessage(string text, params object[] args)
        {
            if (Enabled)
                _log.Add(string.Format(text, args));
        }

        public static IEnumerable<string> GetLogMessages()
        {
            return _log.AsEnumerable();
        }

        private static string GetSecurePath()
        {
            return ConfigurationManager.AppSettings["SecurePath"] ?? "C:\\secure";
        }

        public static void WriteAll(string name, bool debug)
        {
            try
            {
                if (Enabled)
                {
                    var secure = GetSecurePath();
                    var logs = Path.Combine(secure, "logs", name);
                    if (!Directory.Exists(logs)) Directory.CreateDirectory(logs);

                    using (var fs = File.OpenWrite(Path.Combine(logs, "SessionManager.log")))
                    using (var writer = new StreamWriter(fs))
                    {
                        foreach (string line in GetLogMessages())
                        {
                            if (debug)
                                Debug.WriteLine(line);

                            writer.WriteLine(line);
                        }

                        writer.Close();
                    }
                }
            }
            catch { }
        }
    }
}
