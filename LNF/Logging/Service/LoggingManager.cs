using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LNF.Logging.Service
{
    public static class LoggingManager
    {
        public static ServiceResponse AddMessage(ServiceLogMessage msg)
        {
            var client = GetClient();
            if (client != null)
                return client.Add(msg);
            else
                return new ServiceResponse() { Success = false };
        }

        private static IClient GetClient()
        {
            string typeName = ConfigurationManager.AppSettings["LoggingClient"];

            if (string.IsNullOrEmpty(typeName))
                return null;

            Type t = Type.GetType(typeName);

            if (t == null)
                return null;

            var result = (IClient)Activator.CreateInstance(t);

            return result;
        }
    }
}
