using LNF.Impl.DataAccess;
using LNF.Logging;
using System.IO;
using System.Xml.Linq;

namespace LNF.Impl.Logging
{
    public class XmlLoggingService : FileLoggingService
    {
        public XmlLoggingService(ISessionManager mgr) : base(mgr) { }

        protected override string FileExtension => ".xml";

        protected override void AddMessage(LogMessage message)
        {
            var xdoc = GetXmlDocument();

            xdoc.Add(new XElement("message",
                new XAttribute("id", message.MessageID),
                new XElement("timestamp", message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement("level", message.Level),
                new XElement("subject", message.Subject),
                new XElement("body", message.Body),
                message.Data));

            var xml = xdoc.ToString();

            File.WriteAllText(GetLogFilePath(), xml);
        }

        private XElement GetXmlDocument()
        {
            var filePath = GetLogFilePath();

            string xml = string.Empty;

            if (File.Exists(filePath))
                xml = File.ReadAllText(filePath);

            XElement xdoc;

            if (string.IsNullOrWhiteSpace(xml))
                xdoc = new XElement("log", new XAttribute("name", Name));
            else
                xdoc = XElement.Parse(xml);

            return xdoc;
        }
    }
}
