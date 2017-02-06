using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace LNF.Logging
{
    public class XmlLogMessage : FileLogMessage<XElement>
    {
        protected XmlLogMessage(string name, XElement message) : base(name, message) { }

        public static XmlLogMessage Create(string name, XElement message)
        {
            return new XmlLogMessage(name, message);
        }

        protected override string FileExtension
        {
            get { return ".xml"; }
        }

        public override void Write()
        {
            if (Message.Attributes().Any(x => x.Name == "timestamp"))
                Message.Attribute("timestamp").Value = GetCurrentTime();
            else
                Message.Add(new XAttribute("timestamp", GetCurrentTime()));

            var path = GetLogFilePath();

            XDocument xdoc;

            if (File.Exists(path))
                xdoc = XDocument.Load(path);
            else
                xdoc = XDocument.Parse("<root/>");

            xdoc.Root.Add(Message);

            xdoc.Save(path);
        }
    }
}
