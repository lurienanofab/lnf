using LNF.Impl.DataAccess;

namespace LNF.Impl.Logging
{
    public class TextLoggingService : FileLoggingService
    {
        public TextLoggingService(ISessionManager mgr) : base(mgr) { }

        protected override string FileExtension => ".log";
    }
}
