using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml.Linq;

namespace LNF.Logging
{
    public static class Logger
    {
        private static readonly BlockingCollection<ILogMessage> _messages;

        private static bool _enabled;

        public static bool Enabled()
        {
            return _enabled;
        }

        static Logger()
        {
            bool.TryParse(ConfigurationManager.AppSettings["LogEnabled"], out _enabled);

            if (Enabled())
            {
                _messages = new BlockingCollection<ILogMessage>();

                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        var message = _messages.Take();

                        try
                        {
                            message.Write();
                        }
                        catch
                        {
                            //never fail because of logging
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Writes a log message to a file specified by the AppSetting 'LogDirectory' and name.
        /// </summary>
        public static void Write(ILogMessage msg)
        {
            if (Enabled())
                _messages.Add(msg);
        }
    }
}
