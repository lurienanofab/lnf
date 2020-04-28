namespace LNF.Logging
{
    public static class Log
    {
        ///<summary>
        ///Attempts to write to an ILogProvider and silently does nothing if the provider is null.
        ///</summary>
        public static void Write(LogMessageLevel level, string subject, string body, string[] data = null)
        {
            var message = new LogMessage(level, subject, body);

            if (data != null)
            {
                foreach (var s in data)
                    message.AppendData(s);
            }

            Write(message);

        }

        ///<summary>
        ///Attempts to write to an ILogProvider and silently does nothing if the provider is null.
        ///</summary>
        public static void Write(LogMessage message)
        {
            if (ServiceProvider.Current.Log != null)
                ServiceProvider.Current.Log.Write(message);
        }
    }
}
